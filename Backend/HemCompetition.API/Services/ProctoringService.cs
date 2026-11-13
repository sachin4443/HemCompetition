using System.Text.Json;
using HemCompetition.API.Data;
using HemCompetition.API.Domain;
using Microsoft.EntityFrameworkCore;

namespace HemCompetition.API.Services;

public sealed record ViolationOutcome(
    bool Recorded, Guid? EventId, string RuleCode, string Severity, string Action, int Occurrence,
    int? TerminateAfter, int? SuspendAfter, bool CaptureImage, bool CaptureVideo, string Message, string? UploadToken = null);

/// <summary>
/// Central anti-cheating policy. The browser only reports what it observed; counting, escalation,
/// attempt termination and suspension are all decided here from the configured rules.
/// </summary>
public sealed class ProctoringService(
    AppDbContext db, SettingsService settingsService, CompetitionService competitions,
    GameAttemptService games, QuizAttemptService quizzes, BanService bans, TokenService tokens)
{
    /// <summary>Events of the same rule closer together than this are one incident.</summary>
    private static readonly TimeSpan Debounce = TimeSpan.FromSeconds(3);

    public async Task<ViolationOutcome> RecordAsync(long studentId, Guid? authSessionId, string ruleCode, string? message, JsonElement? details)
    {
        ruleCode = ruleCode.Trim().ToUpperInvariant();
        var rule = await db.ViolationRules.AsNoTracking().FirstOrDefaultAsync(r => r.Code == ruleCode);
        if (rule == null || !rule.IsEnabled)
            return new ViolationOutcome(false, null, ruleCode, "", "Ignored", 0, null, null, false, false, "");

        var now = DateTime.UtcNow;
        var competition = await competitions.GetActiveAsync();
        var competitionId = competition?.Id;

        // Dismissed events were judged not to be violations, so they do not count towards escalation.
        var previous = db.ViolationEvents.Where(v => v.StudentId == studentId && v.RuleCode == ruleCode &&
                                                     v.CompetitionId == competitionId && v.ReviewStatus != ReviewStatus.Dismissed);
        var last = await previous.OrderByDescending(v => v.OccurredAtUtc)
            .Select(v => new { v.Id, v.OccurredAtUtc, v.OccurrenceNo, v.ActionTaken }).FirstOrDefaultAsync();
        if (last != null && now - last.OccurredAtUtc < Debounce)
            return new ViolationOutcome(false, last.Id, ruleCode, rule.Severity.ToString(), last.ActionTaken.ToString(),
                last.OccurrenceNo, Limit(rule.TerminateAfter), Limit(rule.SuspendAfter), false, false, "");

        var occurrence = await previous.CountAsync() + 1;
        var session = authSessionId == null ? null : await db.ProctoringSessions
            .Where(p => p.StudentId == studentId && p.AuthSessionId == authSessionId && p.EndedAtUtc == null)
            .OrderByDescending(p => p.StartedAtUtc).FirstOrDefaultAsync();

        var gameAttempt = await db.GameAttempts.Where(a => a.StudentId == studentId && a.Status == AttemptStatus.InProgress)
            .OrderByDescending(a => a.StartedAtUtc).FirstOrDefaultAsync();
        var quizAttempt = await db.QuizAttempts.Where(a => a.StudentId == studentId && a.Status == AttemptStatus.InProgress)
            .OrderByDescending(a => a.StartedAtUtc).FirstOrDefaultAsync();

        var action = rule.SuspendAfter > 0 && occurrence >= rule.SuspendAfter ? ViolationAction.Suspended
            : rule.TerminateAfter > 0 && occurrence >= rule.TerminateAfter ? ViolationAction.Terminated
            : rule.FlagAfter > 0 && occurrence >= rule.FlagAfter ? ViolationAction.Flagged
            : ViolationAction.Warning;

        var evt = new ViolationEvent
        {
            Id = Guid.NewGuid(), StudentId = studentId, CompetitionId = competitionId, ProctoringSessionId = session?.Id,
            RuleCode = ruleCode, Severity = rule.Severity,
            Message = Truncate(string.IsNullOrWhiteSpace(message) ? rule.Name : message, 1000),
            Details = details is { ValueKind: JsonValueKind.Object } d && d.GetRawText().Length <= 4000 ? d.GetRawText() : null,
            GameAttemptId = gameAttempt?.Id, QuizAttemptId = quizAttempt?.Id,
            OccurrenceNo = occurrence, ActionTaken = action, OccurredAtUtc = now, ReviewStatus = ReviewStatus.Open
        };
        db.ViolationEvents.Add(evt);

        await using var tx = await db.Database.BeginTransactionAsync();
        string text;
        switch (action)
        {
            case ViolationAction.Suspended:
                await games.TerminateActiveAsync(studentId);
                await quizzes.TerminateActiveAsync(studentId);
                // Automatic action is always a suspension that an administrator reviews - never a permanent ban.
                await bans.AddAsync(studentId, BanType.Suspension, BanSource.Automatic,
                    $"{rule.Name} (occurrence {occurrence}).",
                    rule.SuspendMinutes == null ? null : now.AddMinutes(rule.SuspendMinutes.Value),
                    competitionId, ruleCode, evt.Id);
                text = "Your access has been suspended pending review. You can request a review after signing in again.";
                break;
            case ViolationAction.Terminated:
                await games.TerminateActiveAsync(studentId);
                await quizzes.TerminateActiveAsync(studentId);
                text = "This attempt has been ended because of repeated rule violations.";
                break;
            case ViolationAction.Flagged:
                if (gameAttempt != null) gameAttempt.IsFlagged = true;
                if (quizAttempt != null) quizAttempt.IsFlagged = true;
                text = $"{rule.Name}. This attempt has been flagged for review.";
                break;
            default:
                text = $"{rule.Name}. Please follow the competition rules.";
                break;
        }
        await db.SaveChangesAsync();
        await tx.CommitAsync();

        var settings = await settingsService.GetAsync();
        var mayCapture = settings.EvidenceCaptureEnabled && session?.EvidenceConsent == true;
        var image = mayCapture && rule.CaptureImage;
        var video = mayCapture && rule.CaptureVideo && settings.VideoEvidenceEnabled && session!.VideoConsent;
        return new ViolationOutcome(true, evt.Id, ruleCode, rule.Severity.ToString(), action.ToString(), occurrence,
            Limit(rule.TerminateAfter), Limit(rule.SuspendAfter), image, video, text,
            image || video ? tokens.EvidenceToken(studentId, evt.Id) : null);
    }

    /// <summary>For violations the server itself detects (for example a second login).</summary>
    public Task<ViolationOutcome> RecordSystemEventAsync(long studentId, string ruleCode, string message) =>
        RecordAsync(studentId, null, ruleCode, message, null);

    private static int? Limit(int value) => value > 0 ? value : null;
    private static string? Truncate(string? s, int max) => s == null || s.Length <= max ? s : s[..max];

    /// <summary>Built-in rules. Created once; administrators own them afterwards.</summary>
    public static ViolationRule[] DefaultRules() =>
    [
        Rule("TAB_SWITCH", "Switched to another tab or window", ViolationSeverity.Minor, image: true, flag: 3, terminate: 8, suspend: 12, minutes: 30),
        Rule("WINDOW_BLUR", "Test window lost focus", ViolationSeverity.Minor, flag: 5),
        Rule("FULLSCREEN_EXIT", "Left fullscreen mode", ViolationSeverity.Minor, image: true, flag: 3, terminate: 8),
        Rule("COPY_PASTE", "Copy, cut or paste attempt", ViolationSeverity.Minor, flag: 5),
        Rule("CONTEXT_MENU", "Right-click attempt", ViolationSeverity.Minor),
        Rule("BLOCKED_KEY", "Blocked keyboard shortcut", ViolationSeverity.Minor, flag: 5),
        Rule("DEVTOOLS", "Developer tools opened", ViolationSeverity.Serious, image: true, flag: 1, terminate: 3),
        Rule("DUPLICATE_TAB", "Competition opened in a second tab", ViolationSeverity.Minor, flag: 2),
        Rule("PAGE_REFRESH", "Page reloaded during an attempt", ViolationSeverity.Minor, flag: 5),
        Rule("NETWORK_LOST", "Connection lost", ViolationSeverity.Minor),
        Rule("MULTIPLE_SESSION", "Signed in while another session was active", ViolationSeverity.Serious, flag: 2),
        Rule("CAMERA_OFF", "Camera stopped or permission withdrawn", ViolationSeverity.Serious, flag: 1, terminate: 4),
        Rule("NO_FACE", "No person visible on camera", ViolationSeverity.Serious, image: true, video: true, flag: 1, terminate: 3, suspend: 5),
        Rule("MULTIPLE_FACES", "More than one person visible on camera", ViolationSeverity.Critical, image: true, video: true, flag: 1, terminate: 2, suspend: 3),
    ];

    private static ViolationRule Rule(string code, string name, ViolationSeverity severity, bool image = false, bool video = false,
        int flag = 0, int terminate = 0, int suspend = 0, int? minutes = null) => new()
    {
        Code = code, Name = name, Severity = severity, IsEnabled = true, CaptureImage = image, CaptureVideo = video,
        FlagAfter = flag, TerminateAfter = terminate, SuspendAfter = suspend, SuspendMinutes = minutes
    };
}
