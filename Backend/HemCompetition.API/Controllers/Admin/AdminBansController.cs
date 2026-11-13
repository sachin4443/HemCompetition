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

public sealed class BanRequest
{
    public long StudentId { get; set; }
    public BanType Type { get; set; }
    [Required, StringLength(2000, MinimumLength = 5)] public string Reason { get; set; } = "";
    /// <summary>Required for a suspension; ignored for a ban.</summary>
    public DateTime? EndsAtUtc { get; set; }
}

public sealed class LiftRequest
{
    [Required, StringLength(2000, MinimumLength = 5)] public string Reason { get; set; } = "";
}

public sealed class AppealNoteRequest
{
    [Required, StringLength(4000, MinimumLength = 2)] public string Note { get; set; } = "";
}

public sealed class AppealDecisionRequest
{
    /// <summary>approve = restore access; reject = keep the restriction; suspend = replace it with a temporary suspension.</summary>
    [Required, RegularExpression("^(approve|reject|suspend)$")] public string Decision { get; set; } = "";
    /// <summary>Recorded and shown to the student.</summary>
    [Required, StringLength(2000, MinimumLength = 5, ErrorMessage = "A reason of at least 5 characters is required.")] public string Reason { get; set; } = "";
    public DateTime? SuspendUntilUtc { get; set; }
}

[ApiController]
[Route("api/admin")]
[Authorize(Policy = Policies.Admin)]
public class AdminBansController(AppDbContext db, AuditService audit, BanService bans) : ControllerBase
{
    // ----------------------------------------------------------------------------- bans

    [HttpGet("bans")]
    [RequirePermission(Permissions.BansView)]
    public async Task<IActionResult> Bans([FromQuery] PageQuery page, [FromQuery] string? state = "active")
    {
        var now = DateTime.UtcNow;
        var q = db.StudentBans.AsNoTracking();
        q = state switch
        {
            "active" => q.Where(b => b.LiftedAtUtc == null && (b.EndsAtUtc == null || b.EndsAtUtc > now)),
            "lifted" => q.Where(b => b.LiftedAtUtc != null),
            "expired" => q.Where(b => b.LiftedAtUtc == null && b.EndsAtUtc <= now),
            _ => q
        };
        var joined = from b in q join u in db.AuthUsers.AsNoTracking() on b.StudentId equals u.UserId select new { b, u };
        if (!string.IsNullOrWhiteSpace(page.Search))
        {
            var s = page.Search.Trim();
            var like = $"%{s}%";
            joined = long.TryParse(s, out var id)
                ? joined.Where(x => x.u.UserId == id || EF.Functions.ILike(x.u.Username, like))
                : joined.Where(x => EF.Functions.ILike(x.u.Username, like) || EF.Functions.ILike(x.u.Email!, like) || EF.Functions.ILike(x.u.FirstName!, like) || EF.Functions.ILike(x.u.LastName!, like));
        }
        var total = await joined.CountAsync();
        var items = await joined.OrderByDescending(x => x.b.StartsAtUtc).Skip(page.Skip).Take(page.PageSize).Select(x => new
        {
            id = x.b.Id, studentId = x.b.StudentId, student = (x.u.FirstName + " " + x.u.LastName).Trim(), username = x.u.Username,
            type = x.b.Type.ToString(), source = x.b.Source.ToString(), reason = x.b.Reason, ruleCode = x.b.RuleCode,
            startsAtUtc = x.b.StartsAtUtc, endsAtUtc = x.b.EndsAtUtc, liftedAtUtc = x.b.LiftedAtUtc, liftReason = x.b.LiftReason,
            isActive = x.b.LiftedAtUtc == null && (x.b.EndsAtUtc == null || x.b.EndsAtUtc > now),
            pendingAppeal = x.b.Appeals.Any(a => a.Status == AppealStatus.Pending)
        }).ToListAsync();
        return Ok(new PagedResult<object>(items, total, page.Page, page.PageSize));
    }

    [HttpPost("bans")]
    [RequirePermission(Permissions.BansManage)]
    public async Task<IActionResult> Create([FromBody] BanRequest model)
    {
        if (!await db.AuthUsers.AnyAsync(u => u.UserId == model.StudentId)) throw ApiException.NotFound("Student not found.");
        DateTime? ends = null;
        if (model.Type == BanType.Suspension)
        {
            ends = model.EndsAtUtc == null ? throw ApiException.BadRequest("A suspension needs an end date and time.") : DateTime.SpecifyKind(model.EndsAtUtc.Value, DateTimeKind.Utc);
            if (ends <= DateTime.UtcNow) throw ApiException.BadRequest("The suspension must end in the future.");
        }
        await using var tx = await db.Database.BeginTransactionAsync();
        var ban = await bans.AddAsync(model.StudentId, model.Type, BanSource.Admin, model.Reason.Trim(), ends, adminId: User.AdminId());
        audit.Add(model.Type == BanType.Ban ? "student.banned" : "student.suspended", "student", model.StudentId, new { banId = ban.Id, model.Reason, ends });
        await db.SaveChangesAsync();
        await tx.CommitAsync();
        return Ok(new { id = ban.Id });
    }

    /// <summary>Restores access. Only an administrator can do this - never the student.</summary>
    [HttpPost("bans/{id:guid}/lift")]
    [RequirePermission(Permissions.BansManage)]
    public async Task<IActionResult> Lift(Guid id, [FromBody] LiftRequest model)
    {
        var ban = await db.StudentBans.FirstOrDefaultAsync(b => b.Id == id) ?? throw ApiException.NotFound();
        if (ban.LiftedAtUtc != null) throw ApiException.Conflict("This restriction has already been lifted.");
        LiftBan(ban, model.Reason.Trim());
        audit.Add("student.ban_lifted", "student", ban.StudentId, new { banId = id, model.Reason });
        await db.SaveChangesAsync();
        return Ok(new { id });
    }

    // ----------------------------------------------------------------------------- appeals

    [HttpGet("appeals")]
    [RequirePermission(Permissions.AppealsView)]
    public async Task<IActionResult> Appeals([FromQuery] PageQuery page, [FromQuery] AppealStatus? status)
    {
        var q = db.Appeals.AsNoTracking();
        if (status != null) q = q.Where(a => a.Status == status);
        var joined = from a in q join u in db.AuthUsers.AsNoTracking() on a.StudentId equals u.UserId select new { a, u };
        if (!string.IsNullOrWhiteSpace(page.Search))
        {
            var s = page.Search.Trim();
            var like = $"%{s}%";
            joined = long.TryParse(s, out var id)
                ? joined.Where(x => x.u.UserId == id || EF.Functions.ILike(x.u.Username, like))
                : joined.Where(x => EF.Functions.ILike(x.u.Username, like) || EF.Functions.ILike(x.u.Email!, like) || EF.Functions.ILike(x.u.EmailId!, like) ||
                                    EF.Functions.ILike(x.u.FirstName!, like) || EF.Functions.ILike(x.u.LastName!, like));
        }
        var total = await joined.CountAsync();
        var items = await joined.OrderBy(x => x.a.Status == AppealStatus.Pending ? 0 : 1).ThenByDescending(x => x.a.SubmittedAtUtc)
            .Skip(page.Skip).Take(page.PageSize).Select(x => new
            {
                id = x.a.Id, banId = x.a.BanId, studentId = x.a.StudentId, student = (x.u.FirstName + " " + x.u.LastName).Trim(),
                username = x.u.Username, email = x.u.EmailId ?? x.u.Email, status = x.a.Status.ToString(), submittedAtUtc = x.a.SubmittedAtUtc,
                decidedAtUtc = x.a.DecidedAtUtc, banType = x.a.Ban.Type.ToString(), banReason = x.a.Ban.Reason
            }).ToListAsync();
        return Ok(new PagedResult<object>(items, total, page.Page, page.PageSize));
    }

    /// <summary>Everything a reviewer needs: the ban, the student's violation history with evidence, and their attempts.</summary>
    [HttpGet("appeals/{id:guid}")]
    [RequirePermission(Permissions.AppealsView)]
    public async Task<IActionResult> Appeal(Guid id)
    {
        var a = await db.Appeals.AsNoTracking().Include(x => x.Ban).Include(x => x.Notes).FirstOrDefaultAsync(x => x.Id == id) ?? throw ApiException.NotFound();
        var u = await db.AuthUsers.AsNoTracking().FirstAsync(x => x.UserId == a.StudentId);
        var decidedBy = a.DecidedByAdminId == null ? null : await db.AdminUsers.Where(x => x.Id == a.DecidedByAdminId).Select(x => x.Username).FirstOrDefaultAsync();
        return Ok(new
        {
            id = a.Id, status = a.Status.ToString(), message = a.Message, submittedAtUtc = a.SubmittedAtUtc,
            decidedAtUtc = a.DecidedAtUtc, decisionReason = a.DecisionReason, decidedBy,
            student = new { userId = u.UserId, name = $"{u.FirstName} {u.LastName}".Trim(), username = u.Username, email = u.EmailId ?? u.Email, phoneNumber = u.PhoneNumber, city = u.City },
            ban = new
            {
                id = a.Ban.Id, type = a.Ban.Type.ToString(), source = a.Ban.Source.ToString(), reason = a.Ban.Reason, ruleCode = a.Ban.RuleCode,
                startsAtUtc = a.Ban.StartsAtUtc, endsAtUtc = a.Ban.EndsAtUtc, liftedAtUtc = a.Ban.LiftedAtUtc, liftReason = a.Ban.LiftReason,
                violationEventId = a.Ban.ViolationEventId
            },
            notes = a.Notes.OrderBy(n => n.CreatedAtUtc).Select(n => new { id = n.Id, admin = n.AdminName, note = n.Note, createdAtUtc = n.CreatedAtUtc }),
            previousAppeals = await db.Appeals.AsNoTracking().Where(x => x.StudentId == a.StudentId && x.Id != id).OrderByDescending(x => x.SubmittedAtUtc)
                .Select(x => new { id = x.Id, status = x.Status.ToString(), submittedAtUtc = x.SubmittedAtUtc, decisionReason = x.DecisionReason }).ToListAsync(),
            violations = await db.ViolationEvents.AsNoTracking().Where(v => v.StudentId == a.StudentId).OrderByDescending(v => v.OccurredAtUtc).Take(200)
                .Select(v => new
                {
                    id = v.Id, ruleCode = v.RuleCode, severity = v.Severity.ToString(), message = v.Message, occurrenceNo = v.OccurrenceNo,
                    action = v.ActionTaken.ToString(), occurredAtUtc = v.OccurredAtUtc, reviewStatus = v.ReviewStatus.ToString(),
                    gameAttemptId = v.GameAttemptId, quizAttemptId = v.QuizAttemptId,
                    evidence = v.Evidence.Where(e => e.DeletedAtUtc == null).Select(e => new { id = e.Id, kind = e.Kind.ToString(), contentType = e.ContentType, capturedAtUtc = e.CapturedAtUtc }).ToList()
                }).AsSplitQuery().ToListAsync(),
            gameAttempts = await db.GameAttempts.AsNoTracking().Where(x => x.StudentId == a.StudentId).OrderByDescending(x => x.StartedAtUtc)
                .Select(x => new { id = x.Id, name = x.Game.Name, status = x.Status.ToString(), score = x.Score, maxScore = x.MaxScore, startedAtUtc = x.StartedAtUtc, isFlagged = x.IsFlagged }).ToListAsync(),
            quizAttempts = await db.QuizAttempts.AsNoTracking().Where(x => x.StudentId == a.StudentId).OrderByDescending(x => x.StartedAtUtc)
                .Select(x => new { id = x.Id, name = x.QuizTitle, status = x.Status.ToString(), score = x.Score, maxScore = x.MaxScore, startedAtUtc = x.StartedAtUtc, isFlagged = x.IsFlagged }).ToListAsync()
        });
    }

    [HttpPost("appeals/{id:guid}/notes")]
    [RequirePermission(Permissions.AppealsReview)]
    public async Task<IActionResult> AddNote(Guid id, [FromBody] AppealNoteRequest model)
    {
        if (!await db.Appeals.AnyAsync(a => a.Id == id)) throw ApiException.NotFound();
        db.AppealNotes.Add(new AppealNote { AppealId = id, AdminId = User.AdminId(), AdminName = User.DisplayName(), Note = model.Note.Trim(), CreatedAtUtc = DateTime.UtcNow });
        audit.Add("appeal.note_added", "appeal", id);
        await db.SaveChangesAsync();
        return Ok(new { id });
    }

    [HttpPost("appeals/{id:guid}/decision")]
    [RequirePermission(Permissions.AppealsReview)]
    public async Task<IActionResult> Decide(Guid id, [FromBody] AppealDecisionRequest model)
    {
        await using var tx = await db.Database.BeginTransactionAsync();
        await db.Database.ExecuteSqlInterpolatedAsync($"SELECT 1 FROM appeals WHERE id = {id} FOR UPDATE");
        var appeal = await db.Appeals.Include(a => a.Ban).FirstOrDefaultAsync(a => a.Id == id) ?? throw ApiException.NotFound();
        if (appeal.Status != AppealStatus.Pending) throw ApiException.Conflict("This appeal has already been decided.", "APPEAL_DECIDED");

        var now = DateTime.UtcNow;
        var reason = model.Reason.Trim();
        var ban = appeal.Ban;
        string outcome;
        switch (model.Decision)
        {
            case "approve":
                appeal.Status = AppealStatus.Approved;
                if (ban.LiftedAtUtc == null) LiftBan(ban, $"Appeal approved: {reason}");
                outcome = "Access restored.";
                break;
            case "suspend":
                var until = model.SuspendUntilUtc == null
                    ? throw ApiException.BadRequest("Choose when the suspension ends.")
                    : DateTime.SpecifyKind(model.SuspendUntilUtc.Value, DateTimeKind.Utc);
                if (until <= now) throw ApiException.BadRequest("The suspension must end in the future.");
                // The appeal is upheld in part: the open-ended restriction becomes a dated suspension.
                appeal.Status = AppealStatus.Approved;
                ban.Type = BanType.Suspension;
                ban.EndsAtUtc = until;
                outcome = $"Restriction changed to a suspension until {until:u}.";
                break;
            default:
                appeal.Status = AppealStatus.Rejected;
                outcome = "Restriction kept.";
                break;
        }
        appeal.DecidedAtUtc = now;
        appeal.DecidedByAdminId = User.AdminId();
        appeal.DecisionReason = reason;
        db.AppealNotes.Add(new AppealNote { AppealId = id, AdminId = User.AdminId(), AdminName = User.DisplayName(), Note = $"Decision: {model.Decision}. {outcome} Reason: {reason}", CreatedAtUtc = now });
        db.Notifications.Add(new Notification
        {
            StudentId = appeal.StudentId, Category = "appeal", CreatedAtUtc = now, CreatedByAdminId = User.AdminId(),
            Title = appeal.Status == AppealStatus.Approved ? "Your review request was accepted" : "Your review request was not accepted",
            Body = $"{outcome} {reason}"
        });
        audit.Add($"appeal.{model.Decision}", "appeal", id, new { appeal.StudentId, banId = ban.Id, reason, model.SuspendUntilUtc });
        await db.SaveChangesAsync();
        await tx.CommitAsync();
        return Ok(new { id, status = appeal.Status.ToString(), outcome });
    }

    private void LiftBan(StudentBan ban, string reason)
    {
        ban.LiftedAtUtc = DateTime.UtcNow;
        ban.LiftedByAdminId = User.AdminId();
        ban.LiftReason = reason;
    }
}
