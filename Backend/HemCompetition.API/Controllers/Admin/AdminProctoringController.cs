using System.ComponentModel.DataAnnotations;
using HemCompetition.API.Data;
using HemCompetition.API.Domain;
using HemCompetition.API.Infrastructure;
using HemCompetition.API.Security;
using HemCompetition.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HemCompetition.API.Controllers.Admin;

public sealed class ViolationReviewRequest
{
    public ReviewStatus Status { get; set; }
    [StringLength(2000)] public string? Note { get; set; }
}

public sealed class ViolationRuleRequest
{
    [Required, StringLength(150)] public string Name { get; set; } = "";
    [StringLength(1000)] public string? Description { get; set; }
    public ViolationSeverity Severity { get; set; }
    public bool IsEnabled { get; set; }
    public bool CaptureImage { get; set; }
    public bool CaptureVideo { get; set; }
    [Range(0, 1000)] public int FlagAfter { get; set; }
    [Range(0, 1000)] public int TerminateAfter { get; set; }
    [Range(0, 1000)] public int SuspendAfter { get; set; }
    [Range(1, 525600)] public int? SuspendMinutes { get; set; }
}

[ApiController]
[Route("api/admin/proctoring")]
[Authorize(Policy = Policies.Admin)]
public class AdminProctoringController(AppDbContext db, AuditService audit, IFileStorage storage) : ControllerBase
{
    /// <summary>Students currently being monitored, with their latest camera state.</summary>
    [HttpGet("live")]
    [RequirePermission(Permissions.ProctoringView)]
    public async Task<IActionResult> Live()
    {
        var since = DateTime.UtcNow.AddMinutes(-5);
        var sessions = await (from p in db.ProctoringSessions.AsNoTracking().Where(p => p.EndedAtUtc == null && p.LastSeenAtUtc >= since)
            join u in db.AuthUsers.AsNoTracking() on p.StudentId equals u.UserId
            orderby p.LastSeenAtUtc descending
            select new
            {
                sessionId = p.Id, studentId = p.StudentId, student = (u.FirstName + " " + u.LastName).Trim(), username = u.Username,
                startedAtUtc = p.StartedAtUtc, lastSeenAtUtc = p.LastSeenAtUtc, cameraStatus = p.LastCameraStatus, personCount = p.LastPersonCount,
                evidenceConsent = p.EvidenceConsent, videoConsent = p.VideoConsent,
                violations = db.ViolationEvents.Count(v => v.ProctoringSessionId == p.Id),
                activity = db.QuizAttempts.Where(a => a.StudentId == p.StudentId && a.Status == AttemptStatus.InProgress).Select(a => a.QuizTitle).FirstOrDefault()
                           ?? db.GameAttempts.Where(a => a.StudentId == p.StudentId && a.Status == AttemptStatus.InProgress).Select(a => a.Game.Name).FirstOrDefault()
            }).Take(500).ToListAsync();
        return Ok(sessions);
    }

    [HttpGet("violations")]
    [RequirePermission(Permissions.ProctoringView)]
    public async Task<IActionResult> Violations([FromQuery] PageQuery page, [FromQuery] ReviewStatus? review, [FromQuery] ViolationSeverity? severity,
        [FromQuery] string? ruleCode, [FromQuery] long? studentId, [FromQuery] Guid? competitionId)
    {
        var q = db.ViolationEvents.AsNoTracking();
        if (review != null) q = q.Where(v => v.ReviewStatus == review);
        if (severity != null) q = q.Where(v => v.Severity == severity);
        if (!string.IsNullOrWhiteSpace(ruleCode)) q = q.Where(v => v.RuleCode == ruleCode);
        if (studentId != null) q = q.Where(v => v.StudentId == studentId);
        if (competitionId != null) q = q.Where(v => v.CompetitionId == competitionId);
        var joined = from v in q join u in db.AuthUsers.AsNoTracking() on v.StudentId equals u.UserId select new { v, u };
        if (!string.IsNullOrWhiteSpace(page.Search))
        {
            var like = $"%{page.Search.Trim()}%";
            joined = joined.Where(x => EF.Functions.ILike(x.u.Username, like) || EF.Functions.ILike(x.u.FirstName!, like) || EF.Functions.ILike(x.u.LastName!, like));
        }
        var total = await joined.CountAsync();
        var items = await joined.OrderByDescending(x => x.v.OccurredAtUtc).Skip(page.Skip).Take(page.PageSize).Select(x => new
        {
            id = x.v.Id, studentId = x.v.StudentId, student = (x.u.FirstName + " " + x.u.LastName).Trim(), username = x.u.Username,
            ruleCode = x.v.RuleCode, severity = x.v.Severity.ToString(), message = x.v.Message, occurrenceNo = x.v.OccurrenceNo,
            action = x.v.ActionTaken.ToString(), occurredAtUtc = x.v.OccurredAtUtc, reviewStatus = x.v.ReviewStatus.ToString(),
            evidenceCount = x.v.Evidence.Count(e => e.DeletedAtUtc == null), gameAttemptId = x.v.GameAttemptId, quizAttemptId = x.v.QuizAttemptId
        }).ToListAsync();
        return Ok(new PagedResult<object>(items, total, page.Page, page.PageSize));
    }

    [HttpGet("violations/{id:guid}")]
    [RequirePermission(Permissions.ProctoringView)]
    public async Task<IActionResult> Violation(Guid id)
    {
        var v = await db.ViolationEvents.AsNoTracking().Include(x => x.Evidence).FirstOrDefaultAsync(x => x.Id == id) ?? throw ApiException.NotFound();
        var u = await db.AuthUsers.AsNoTracking().FirstAsync(x => x.UserId == v.StudentId);
        return Ok(new
        {
            id = v.Id, studentId = v.StudentId, student = $"{u.FirstName} {u.LastName}".Trim(), username = u.Username,
            ruleCode = v.RuleCode, severity = v.Severity.ToString(), message = v.Message,
            details = v.Details == null ? null : System.Text.Json.Nodes.JsonNode.Parse(v.Details),
            occurrenceNo = v.OccurrenceNo, action = v.ActionTaken.ToString(), occurredAtUtc = v.OccurredAtUtc,
            reviewStatus = v.ReviewStatus.ToString(), reviewNote = v.ReviewNote, reviewedAtUtc = v.ReviewedAtUtc,
            gameAttemptId = v.GameAttemptId, quizAttemptId = v.QuizAttemptId,
            evidence = v.Evidence.OrderBy(e => e.CapturedAtUtc).Select(EvidenceShape)
        });
    }

    /// <summary>Confirms or dismisses a violation. Dismissed events no longer count towards escalation.</summary>
    [HttpPost("violations/{id:guid}/review")]
    [RequirePermission(Permissions.ViolationsReview)]
    public async Task<IActionResult> Review(Guid id, [FromBody] ViolationReviewRequest model)
    {
        var v = await db.ViolationEvents.FirstOrDefaultAsync(x => x.Id == id) ?? throw ApiException.NotFound();
        v.ReviewStatus = model.Status;
        v.ReviewNote = string.IsNullOrWhiteSpace(model.Note) ? null : model.Note.Trim();
        v.ReviewedByAdminId = User.AdminId();
        v.ReviewedAtUtc = DateTime.UtcNow;
        audit.Add("violation.reviewed", "violation", id, new { status = model.Status.ToString(), v.StudentId });
        await db.SaveChangesAsync();
        return Ok(new { id, reviewStatus = v.ReviewStatus.ToString() });
    }

    // ----------------------------------------------------------------------------- rules

    [HttpGet("rules")]
    [RequirePermission(Permissions.ProctoringView)]
    public async Task<IActionResult> Rules() =>
        Ok(await db.ViolationRules.AsNoTracking().OrderBy(r => r.Id).Select(r => new
        {
            id = r.Id, code = r.Code, name = r.Name, description = r.Description, severity = r.Severity.ToString(), isEnabled = r.IsEnabled,
            captureImage = r.CaptureImage, captureVideo = r.CaptureVideo, flagAfter = r.FlagAfter, terminateAfter = r.TerminateAfter,
            suspendAfter = r.SuspendAfter, suspendMinutes = r.SuspendMinutes
        }).ToListAsync());

    [HttpPut("rules/{id:int}")]
    [RequirePermission(Permissions.ViolationsReview)]
    public async Task<IActionResult> UpdateRule(int id, [FromBody] ViolationRuleRequest model)
    {
        var r = await db.ViolationRules.FirstOrDefaultAsync(x => x.Id == id) ?? throw ApiException.NotFound();
        r.Name = model.Name.Trim();
        r.Description = string.IsNullOrWhiteSpace(model.Description) ? null : model.Description.Trim();
        r.Severity = model.Severity;
        r.IsEnabled = model.IsEnabled;
        r.CaptureImage = model.CaptureImage;
        r.CaptureVideo = model.CaptureVideo;
        r.FlagAfter = model.FlagAfter;
        r.TerminateAfter = model.TerminateAfter;
        r.SuspendAfter = model.SuspendAfter;
        r.SuspendMinutes = model.SuspendMinutes;
        r.UpdatedAtUtc = DateTime.UtcNow;
        audit.Add("violation_rule.updated", "violation_rule", r.Code, model);
        await db.SaveChangesAsync();
        return Ok(new { id });
    }

    // ----------------------------------------------------------------------------- evidence

    [HttpGet("evidence")]
    [RequirePermission(Permissions.EvidenceView)]
    public async Task<IActionResult> Evidence([FromQuery] PageQuery page, [FromQuery] long? studentId, [FromQuery] EvidenceKind? kind, [FromQuery] bool includeDeleted = false)
    {
        var q = db.EvidenceFiles.AsNoTracking();
        if (!includeDeleted) q = q.Where(e => e.DeletedAtUtc == null);
        if (studentId != null) q = q.Where(e => e.StudentId == studentId);
        if (kind != null) q = q.Where(e => e.Kind == kind);
        var joined = from e in q join u in db.AuthUsers.AsNoTracking() on e.StudentId equals u.UserId select new { e, u };
        if (!string.IsNullOrWhiteSpace(page.Search))
        {
            var like = $"%{page.Search.Trim()}%";
            joined = joined.Where(x => EF.Functions.ILike(x.u.Username, like) || EF.Functions.ILike(x.u.FirstName!, like) || EF.Functions.ILike(x.u.LastName!, like));
        }
        var total = await joined.CountAsync();
        var items = await joined.OrderByDescending(x => x.e.CapturedAtUtc).Skip(page.Skip).Take(page.PageSize).Select(x => new
        {
            id = x.e.Id, violationEventId = x.e.ViolationEventId, ruleCode = x.e.ViolationEvent.RuleCode, studentId = x.e.StudentId,
            student = (x.u.FirstName + " " + x.u.LastName).Trim(), username = x.u.Username, kind = x.e.Kind.ToString(),
            contentType = x.e.ContentType, sizeBytes = x.e.SizeBytes, capturedAtUtc = x.e.CapturedAtUtc, retainUntilUtc = x.e.RetainUntilUtc,
            deletedAtUtc = x.e.DeletedAtUtc
        }).ToListAsync();
        return Ok(new PagedResult<object>(items, total, page.Page, page.PageSize));
    }

    /// <summary>Streams one evidence file. Every view is recorded in the audit log.</summary>
    [HttpGet("evidence/{id:guid}/content")]
    [RequirePermission(Permissions.EvidenceView)]
    public async Task<IActionResult> EvidenceContent(Guid id)
    {
        var e = await db.EvidenceFiles.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id && x.DeletedAtUtc == null) ?? throw ApiException.NotFound();
        var stream = await storage.OpenAsync(e.StorageKey) ?? throw ApiException.NotFound("The file is no longer in storage.");
        await audit.LogAsync("evidence.viewed", "evidence", id, new { e.StudentId });
        Response.Headers.ContentDisposition = "inline";
        return File(stream, e.ContentType, enableRangeProcessing: true);
    }

    [HttpDelete("evidence/{id:guid}")]
    [RequirePermission(Permissions.EvidenceDelete)]
    public async Task<IActionResult> DeleteEvidence(Guid id)
    {
        var e = await db.EvidenceFiles.FirstOrDefaultAsync(x => x.Id == id && x.DeletedAtUtc == null) ?? throw ApiException.NotFound();
        await storage.DeleteAsync(e.StorageKey);
        e.DeletedAtUtc = DateTime.UtcNow;
        e.DeletedBy = User.DisplayName();
        audit.Add("evidence.deleted", "evidence", id, new { e.StudentId });
        await db.SaveChangesAsync();
        return Ok(new { id });
    }

    private static object EvidenceShape(EvidenceFile e) => new
    {
        id = e.Id, kind = e.Kind.ToString(), contentType = e.ContentType, sizeBytes = e.SizeBytes, sha256 = e.Sha256,
        capturedAtUtc = e.CapturedAtUtc, retainUntilUtc = e.RetainUntilUtc, deletedAtUtc = e.DeletedAtUtc, deletedBy = e.DeletedBy
    };
}
