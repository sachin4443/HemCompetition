using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using HemCompetition.API.Data;
using HemCompetition.API.Domain;
using HemCompetition.API.Infrastructure;
using HemCompetition.API.Security;
using HemCompetition.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;

namespace HemCompetition.API.Controllers;

public sealed class ConsentRequest
{
    /// <summary>Must be true: the competition rules and camera monitoring were read and accepted.</summary>
    public bool AcceptTerms { get; set; }
    /// <summary>Agrees to snapshots being captured when a violation is detected.</summary>
    public bool AcceptEvidenceCapture { get; set; }
    /// <summary>Optional: agrees to short video clips as well.</summary>
    public bool AcceptVideoCapture { get; set; }
}

public sealed class ViolationRequest
{
    [Required, StringLength(50)] public string RuleCode { get; set; } = string.Empty;
    [StringLength(500)] public string? Message { get; set; }
    public JsonElement? Details { get; set; }
}

public sealed class CameraStatusRequest
{
    [StringLength(50)] public string? CameraStatus { get; set; }
    [Range(0, 50)] public int PersonCount { get; set; }
}

[ApiController]
[Route("api/proctoring")]
[Authorize(Policy = Policies.Student)]
public class ProctoringController(
    AppDbContext db, ProctoringService proctoring, SettingsService settingsService,
    CompetitionService competitions, IFileStorage storage) : ControllerBase
{
    /// <summary>What the student must agree to and what the browser should monitor.</summary>
    [HttpGet("status")]
    public async Task<IActionResult> Status()
    {
        var studentId = User.StudentId();
        var settings = await settingsService.GetAsync();
        var competition = await competitions.GetActiveAsync();
        var security = await db.CompetitionSecurities.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == studentId);
        var session = await CurrentSession(studentId, tracking: false);
        var required = competition?.ProctoringRequired ?? true;
        return Ok(new
        {
            proctoringRequired = required,
            cameraRequired = required && settings.CameraRequired,
            fullscreenRequired = required && settings.FullscreenRequired,
            consentVersion = settings.ConsentVersion,
            termsAccepted = security is { TermsAccepted: true } && security.ConsentVersion == settings.ConsentVersion,
            sessionActive = session != null,
            evidenceConsent = session?.EvidenceConsent ?? false,
            videoConsent = session?.VideoConsent ?? false,
            evidenceCaptureEnabled = settings.EvidenceCaptureEnabled,
            videoEvidenceEnabled = settings.EvidenceCaptureEnabled && settings.VideoEvidenceEnabled,
            videoClipSeconds = settings.VideoClipSeconds,
            evidenceRetentionDays = settings.EvidenceRetentionDays,
            noFaceSeconds = settings.NoFaceSeconds,
            multipleFacesSeconds = settings.MultipleFacesSeconds,
            rules = await db.ViolationRules.AsNoTracking().Where(r => r.IsEnabled).OrderBy(r => r.Id)
                .Select(r => new { code = r.Code, name = r.Name, severity = r.Severity.ToString(),
                    terminateAfter = r.TerminateAfter, suspendAfter = r.SuspendAfter }).ToListAsync()
        });
    }

    /// <summary>Records explicit consent and opens the monitoring session for this login.</summary>
    [HttpPost("consent")]
    public async Task<IActionResult> Consent([FromBody] ConsentRequest model)
    {
        if (!model.AcceptTerms) throw ApiException.BadRequest("You must accept the competition rules to continue.");
        var studentId = User.StudentId();
        var settings = await settingsService.GetAsync();
        var competition = await competitions.GetActiveAsync();
        var now = DateTime.UtcNow;

        var security = await db.CompetitionSecurities.FirstOrDefaultAsync(x => x.UserId == studentId);
        if (security == null)
            db.CompetitionSecurities.Add(security = new CompetitionSecurity { Id = Guid.NewGuid(), UserId = studentId, Status = "ACTIVE", CreatedAtUtc = now });
        security.ConsentVersion = settings.ConsentVersion;
        security.TermsAccepted = true;
        security.TermsAcceptedAtUtc = now;
        security.UpdatedAtUtc = now;

        var session = await CurrentSession(studentId, tracking: true);
        if (session == null)
            db.ProctoringSessions.Add(session = new ProctoringSession
            {
                Id = Guid.NewGuid(), StudentId = studentId, CompetitionId = competition?.Id, AuthSessionId = User.SessionId(),
                StartedAtUtc = now, IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                UserAgent = Request.Headers.UserAgent.ToString() is { Length: > 1000 } ua ? ua[..1000] : Request.Headers.UserAgent.ToString()
            });
        session.ConsentVersion = settings.ConsentVersion;
        session.EvidenceConsent = model.AcceptEvidenceCapture;
        session.VideoConsent = model.AcceptEvidenceCapture && model.AcceptVideoCapture;
        session.LastSeenAtUtc = now;
        await db.SaveChangesAsync();
        return Ok(new { sessionId = session.Id, termsAccepted = true, evidenceConsent = session.EvidenceConsent, videoConsent = session.VideoConsent });
    }

    [HttpPost("camera-status")]
    public async Task<IActionResult> CameraStatus([FromBody] CameraStatusRequest model)
    {
        var studentId = User.StudentId();
        var session = await CurrentSession(studentId, tracking: true);
        if (session == null) return Ok(new { sessionActive = false });
        session.LastCameraStatus = model.CameraStatus;
        session.LastPersonCount = model.PersonCount;
        session.LastSeenAtUtc = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return Ok(new { sessionActive = true });
    }

    /// <summary>Reports an observed violation. The response says what the server decided.</summary>
    [HttpPost("events")]
    [EnableRateLimiting("proctoring")]
    public async Task<IActionResult> Report([FromBody] ViolationRequest model)
    {
        var outcome = await proctoring.RecordAsync(User.StudentId(), User.SessionId(), model.RuleCode, model.Message, model.Details);
        return Ok(outcome);
    }

    /// <summary>Uploads a snapshot or short clip for a violation the server just recorded.</summary>
    [HttpPost("events/{eventId:guid}/evidence")]
    [EnableRateLimiting("proctoring")]
    [RequestSizeLimit(60_000_000)]
    public async Task<IActionResult> UploadEvidence(Guid eventId, IFormFile file)
    {
        var studentId = User.StudentId();
        var settings = await settingsService.GetAsync();
        if (!settings.EvidenceCaptureEnabled) throw ApiException.Forbidden("Evidence capture is turned off.");

        var evt = await db.ViolationEvents.AsNoTracking().FirstOrDefaultAsync(v => v.Id == eventId && v.StudentId == studentId)
                  ?? throw ApiException.NotFound();
        // Evidence must be captured at the time of the event, not supplied later.
        if (DateTime.UtcNow - evt.OccurredAtUtc > TimeSpan.FromMinutes(3)) throw ApiException.Conflict("This event is closed for evidence.");
        if (await db.EvidenceFiles.CountAsync(e => e.ViolationEventId == eventId) >= 3) throw ApiException.Conflict("Evidence limit reached for this event.");

        var session = evt.ProctoringSessionId == null ? null
            : await db.ProctoringSessions.AsNoTracking().FirstOrDefaultAsync(p => p.Id == evt.ProctoringSessionId);
        if (session is not { EvidenceConsent: true }) throw ApiException.Forbidden("Consent for evidence capture has not been given.");

        var rule = await db.ViolationRules.AsNoTracking().FirstOrDefaultAsync(r => r.Code == evt.RuleCode);
        var allowed = new List<string>();
        if (rule?.CaptureImage == true) allowed.AddRange(["image/jpeg", "image/png", "image/webp"]);
        if (rule?.CaptureVideo == true && settings.VideoEvidenceEnabled && session.VideoConsent) allowed.AddRange(["video/webm", "video/mp4"]);
        if (allowed.Count == 0) throw ApiException.Forbidden("This kind of event does not accept evidence.");

        var maxBytes = 1024L * Math.Max(settings.MaxEvidenceImageKb, settings.MaxEvidenceVideoKb);
        var (content, contentType) = await FileSniffer.ReadVerifiedAsync(file, maxBytes, allowed);
        await using var _ = content;
        var kind = contentType.StartsWith("video/") ? EvidenceKind.Video : EvidenceKind.Image;
        var limit = 1024L * (kind == EvidenceKind.Video ? settings.MaxEvidenceVideoKb : settings.MaxEvidenceImageKb);
        if (content.Length > limit) throw new ApiException(413, "The file is too large.", "FILE_TOO_LARGE");

        var id = Guid.NewGuid();
        var key = $"evidence/{evt.OccurredAtUtc:yyyy/MM/dd}/{id:N}{FileSniffer.Extension(contentType)}";
        var sha = await storage.SaveAsync(key, content);
        db.EvidenceFiles.Add(new EvidenceFile
        {
            Id = id, ViolationEventId = eventId, StudentId = studentId, Kind = kind, StorageKey = key, ContentType = contentType,
            SizeBytes = content.Length, Sha256 = sha, CapturedAtUtc = DateTime.UtcNow,
            RetainUntilUtc = DateTime.UtcNow.AddDays(settings.EvidenceRetentionDays)
        });
        await db.SaveChangesAsync();
        return Ok(new { id, kind = kind.ToString() });
    }

    [HttpPost("end")]
    public async Task<IActionResult> End()
    {
        var session = await CurrentSession(User.StudentId(), tracking: true);
        if (session != null)
        {
            session.EndedAtUtc = DateTime.UtcNow;
            await db.SaveChangesAsync();
        }
        return Ok(new { ended = true });
    }

    private Task<ProctoringSession?> CurrentSession(long studentId, bool tracking)
    {
        var authSession = User.SessionId();
        var q = db.ProctoringSessions.Where(p => p.StudentId == studentId && p.AuthSessionId == authSession && p.EndedAtUtc == null);
        if (!tracking) q = q.AsNoTracking();
        return q.OrderByDescending(p => p.StartedAtUtc).FirstOrDefaultAsync();
    }
}
