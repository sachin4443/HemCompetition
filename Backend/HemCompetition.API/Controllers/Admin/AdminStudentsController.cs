using System.ComponentModel.DataAnnotations;
using HemCompetition.API.Data;
using HemCompetition.API.Domain;
using HemCompetition.API.Infrastructure;
using HemCompetition.API.Security;
using HemCompetition.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HemCompetition.API.Controllers.Admin;

public sealed class StudentRequest
{
    [Required, StringLength(256, MinimumLength = 3)] public string Username { get; set; } = "";
    [StringLength(200)] public string? Password { get; set; }
    [StringLength(200)] public string? FirstName { get; set; }
    [StringLength(200)] public string? LastName { get; set; }
    [EmailAddress, StringLength(256)] public string? Email { get; set; }
    [StringLength(50)] public string? PhoneNumber { get; set; }
    [StringLength(200)] public string? City { get; set; }
    public bool IsActive { get; set; } = true;
}

public sealed class PasswordResetRequest
{
    [Required, StringLength(200, MinimumLength = 8)] public string NewPassword { get; set; } = "";
}

public sealed class NotificationRequest
{
    public long? StudentId { get; set; }
    [Required, StringLength(200)] public string Title { get; set; } = "";
    [Required, StringLength(4000)] public string Body { get; set; } = "";
}

[ApiController]
[Route("api/admin/students")]
[Authorize(Policy = Policies.Admin)]
public class AdminStudentsController(AppDbContext db, AuditService audit, SessionService sessions, BanService bans) : ControllerBase
{
    private static readonly PasswordHasher<AuthUser> Hasher = new();

    [HttpGet]
    [RequirePermission(Permissions.StudentsView)]
    public async Task<IActionResult> List([FromQuery] PageQuery page, [FromQuery] string? status = null)
    {
        var now = DateTime.UtcNow;
        var q = db.AuthUsers.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(page.Search))
        {
            var s = page.Search.Trim();
            var like = $"%{s}%";
            q = long.TryParse(s, out var id)
                ? q.Where(u => u.UserId == id || EF.Functions.ILike(u.Username, like) || EF.Functions.ILike(u.PhoneNumber!, like))
                : q.Where(u => EF.Functions.ILike(u.Username, like) || EF.Functions.ILike(u.Email!, like) || EF.Functions.ILike(u.EmailId!, like) ||
                               EF.Functions.ILike(u.FirstName!, like) || EF.Functions.ILike(u.LastName!, like) || EF.Functions.ILike(u.City!, like));
        }
        var activeBans = db.StudentBans.Where(b => b.LiftedAtUtc == null && b.StartsAtUtc <= now && (b.EndsAtUtc == null || b.EndsAtUtc > now));
        q = status switch
        {
            "active" => q.Where(u => u.IsActive && !u.IsUserDeactivated),
            "inactive" => q.Where(u => !u.IsActive || u.IsUserDeactivated),
            "restricted" => q.Where(u => activeBans.Any(b => b.StudentId == u.UserId)),
            "online" => q.Where(u => db.AuthSessions.Any(s => s.UserId == u.UserId && s.IsActive)),
            _ => q
        };
        var total = await q.CountAsync();
        var items = await q.OrderBy(u => u.UserId).Skip(page.Skip).Take(page.PageSize)
            .Select(u => new
            {
                userId = u.UserId, username = u.Username, firstName = u.FirstName, lastName = u.LastName,
                email = u.EmailId ?? u.Email, phoneNumber = u.PhoneNumber, city = u.City,
                isActive = u.IsActive && !u.IsUserDeactivated, lastLoginAtUtc = u.LastLoginAtUtc,
                restricted = activeBans.Any(b => b.StudentId == u.UserId),
                online = db.AuthSessions.Any(s => s.UserId == u.UserId && s.IsActive)
            }).ToListAsync();
        return Ok(new PagedResult<object>(items, total, page.Page, page.PageSize));
    }

    /// <summary>One student with their attempts, violations and restrictions.</summary>
    [HttpGet("{userId:long}")]
    [RequirePermission(Permissions.StudentsView)]
    public async Task<IActionResult> Get(long userId)
    {
        var u = await db.AuthUsers.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == userId) ?? throw ApiException.NotFound("Student not found.");
        var ban = await bans.GetActiveAsync(userId);
        return Ok(new
        {
            profile = new
            {
                userId = u.UserId, username = u.Username, firstName = u.FirstName, lastName = u.LastName,
                email = u.EmailId ?? u.Email, phoneNumber = u.PhoneNumber, address = u.AddressLine1, city = u.City, pincode = u.Pincode,
                isActive = u.IsActive && !u.IsUserDeactivated, createdAtUtc = u.CreatedAtUtc, lastLoginAtUtc = u.LastLoginAtUtc,
                lockedUntilUtc = u.LockoutEndDateUtc > DateTime.UtcNow ? u.LockoutEndDateUtc : null
            },
            activeBan = ban == null ? null : BanService.Describe(ban),
            gameAttempts = await db.GameAttempts.AsNoTracking().Where(a => a.StudentId == userId).OrderByDescending(a => a.StartedAtUtc)
                .Select(a => new { id = a.Id, game = a.Game.Name, status = a.Status.ToString(), score = a.Score, maxScore = a.MaxScore,
                    correct = a.CorrectCount, total = a.TotalRounds, timeTakenSeconds = a.TimeTakenSeconds, startedAtUtc = a.StartedAtUtc, isFlagged = a.IsFlagged }).ToListAsync(),
            quizAttempts = await db.QuizAttempts.AsNoTracking().Where(a => a.StudentId == userId).OrderByDescending(a => a.StartedAtUtc)
                .Select(a => new { id = a.Id, quiz = a.QuizTitle, status = a.Status.ToString(), score = a.Score, maxScore = a.MaxScore,
                    correct = a.CorrectCount, incorrect = a.IncorrectCount, unanswered = a.UnansweredCount,
                    timeTakenSeconds = a.TimeTakenSeconds, startedAtUtc = a.StartedAtUtc, isFlagged = a.IsFlagged }).ToListAsync(),
            violations = await db.ViolationEvents.AsNoTracking().Where(v => v.StudentId == userId).OrderByDescending(v => v.OccurredAtUtc).Take(100)
                .Select(v => new { id = v.Id, ruleCode = v.RuleCode, severity = v.Severity.ToString(), message = v.Message,
                    action = v.ActionTaken.ToString(), occurredAtUtc = v.OccurredAtUtc, reviewStatus = v.ReviewStatus.ToString(), evidenceCount = v.Evidence.Count(e => e.DeletedAtUtc == null) }).ToListAsync(),
            bans = await db.StudentBans.AsNoTracking().Where(b => b.StudentId == userId).OrderByDescending(b => b.StartsAtUtc)
                .Select(b => new { id = b.Id, type = b.Type.ToString(), source = b.Source.ToString(), reason = b.Reason, startsAtUtc = b.StartsAtUtc,
                    endsAtUtc = b.EndsAtUtc, liftedAtUtc = b.LiftedAtUtc, liftReason = b.LiftReason }).ToListAsync()
        });
    }

    [HttpPost]
    [RequirePermission(Permissions.StudentsManage)]
    public async Task<IActionResult> Create([FromBody] StudentRequest model)
    {
        if (string.IsNullOrWhiteSpace(model.Password) || model.Password.Length < 8)
            throw ApiException.BadRequest("A password of at least 8 characters is required.");
        var username = model.Username.Trim();
        if (await db.AuthUsers.AnyAsync(u => u.Username == username)) throw ApiException.Conflict("That username is already in use.");
        var user = NewUser(await NextUserIdAsync(), username, model.Password);
        Apply(user, model);
        db.AuthUsers.Add(user);
        audit.Add("student.created", "student", user.UserId, new { user.Username });
        await db.SaveChangesAsync();
        return Ok(new { userId = user.UserId });
    }

    [HttpPut("{userId:long}")]
    [RequirePermission(Permissions.StudentsManage)]
    public async Task<IActionResult> Update(long userId, [FromBody] StudentRequest model)
    {
        var user = await db.AuthUsers.FirstOrDefaultAsync(u => u.UserId == userId) ?? throw ApiException.NotFound("Student not found.");
        var username = model.Username.Trim();
        if (username != user.Username && await db.AuthUsers.AnyAsync(u => u.Username == username))
            throw ApiException.Conflict("That username is already in use.");
        var wasActive = user.IsActive && !user.IsUserDeactivated;
        user.Username = username;
        Apply(user, model);
        user.UpdatedAtUtc = DateTime.UtcNow;
        audit.Add("student.updated", "student", userId, new { model.IsActive });
        await db.SaveChangesAsync();
        if (wasActive && !model.IsActive) await sessions.TerminateAsync(userId);
        return Ok(new { userId });
    }

    [HttpPost("{userId:long}/reset-password")]
    [RequirePermission(Permissions.StudentsManage)]
    public async Task<IActionResult> ResetPassword(long userId, [FromBody] PasswordResetRequest model)
    {
        var user = await db.AuthUsers.FirstOrDefaultAsync(u => u.UserId == userId) ?? throw ApiException.NotFound("Student not found.");
        user.PasswordHash = Hasher.HashPassword(user, model.NewPassword);
        user.SecurityStamp = Guid.NewGuid().ToString();
        user.AccessFailedCount = 0;
        user.LockoutEndDateUtc = null;
        user.UpdatedAtUtc = DateTime.UtcNow;
        audit.Add("student.password_reset", "student", userId);
        await db.SaveChangesAsync();
        await sessions.TerminateAsync(userId);
        return Ok(new { message = "Password reset. The student has been signed out." });
    }

    [HttpPost("{userId:long}/terminate-session")]
    [RequirePermission(Permissions.SessionsManage)]
    public async Task<IActionResult> TerminateSession(long userId)
    {
        await sessions.TerminateAsync(userId);
        await audit.LogAsync("student.session_terminated", "student", userId);
        return Ok(new { message = "Session ended." });
    }

    /// <summary>Bulk import. Columns: username, password, first_name, last_name, email, phone, city, user_id (optional).</summary>
    [HttpPost("import")]
    [RequirePermission(Permissions.StudentsManage)]
    [RequestSizeLimit(6_000_000)]
    public async Task<IActionResult> Import(IFormFile file)
    {
        var table = await TabularFile.ReadAsync(file);
        int cUser = table.Column("username", "user", "login", "mobile"), cPass = table.Column("password"),
            cFirst = table.Column("firstname", "first"), cLast = table.Column("lastname", "last"),
            cEmail = table.Column("email", "emailid"), cPhone = table.Column("phone", "phonenumber", "mobileno"),
            cCity = table.Column("city"), cId = table.Column("userid", "id");
        if (cUser < 0 || cPass < 0) throw ApiException.BadRequest("The file needs 'username' and 'password' columns.");

        var existingNames = (await db.AuthUsers.Select(u => u.Username).ToListAsync()).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var existingIds = (await db.AuthUsers.Select(u => u.UserId).ToListAsync()).ToHashSet();
        var nextId = await NextUserIdAsync();
        var errors = new List<string>();
        var users = new List<AuthUser>();
        var skipped = 0;
        for (var i = 0; i < table.Rows.Count; i++)
        {
            var row = table.Rows[i];
            var line = i + 2;
            var username = TabularFile.Table.Cell(row, cUser);
            var password = TabularFile.Table.Cell(row, cPass);
            if (username.Length < 3 || username.Length > 256) { errors.Add($"Row {line}: username must be 3-256 characters."); continue; }
            if (!existingNames.Add(username)) { skipped++; continue; }
            if (password.Length < 8) { errors.Add($"Row {line}: password must be at least 8 characters."); continue; }
            long userId;
            var idRaw = TabularFile.Table.Cell(row, cId);
            if (idRaw.Length > 0)
            {
                if (!long.TryParse(idRaw, out userId) || userId <= 0) { errors.Add($"Row {line}: user_id must be a positive number."); continue; }
                if (!existingIds.Add(userId)) { errors.Add($"Row {line}: user_id {userId} is already in use."); continue; }
            }
            else
            {
                while (!existingIds.Add(nextId)) nextId++;
                userId = nextId++;
            }
            var user = NewUser(userId, username, password);
            user.FirstName = Null(TabularFile.Table.Cell(row, cFirst));
            user.LastName = Null(TabularFile.Table.Cell(row, cLast));
            user.Email = user.EmailId = Null(TabularFile.Table.Cell(row, cEmail));
            user.PhoneNumber = Null(TabularFile.Table.Cell(row, cPhone));
            user.City = Null(TabularFile.Table.Cell(row, cCity));
            users.Add(user);
        }
        if (errors.Count > 0) return BadRequest(new { message = "Nothing was imported. Fix these rows and try again.", errors = errors.Take(50) });
        db.AuthUsers.AddRange(users);
        audit.Add("student.imported", "student", null, new { count = users.Count, skipped });
        await db.SaveChangesAsync();
        return Ok(new { imported = users.Count, skipped });
    }

    [HttpPost("notifications")]
    [RequirePermission(Permissions.NotificationsSend)]
    public async Task<IActionResult> Notify([FromBody] NotificationRequest model)
    {
        if (model.StudentId != null && !await db.AuthUsers.AnyAsync(u => u.UserId == model.StudentId))
            throw ApiException.NotFound("Student not found.");
        db.Notifications.Add(new Notification
        {
            StudentId = model.StudentId, Title = model.Title.Trim(), Body = model.Body.Trim(),
            Category = "announcement", CreatedAtUtc = DateTime.UtcNow, CreatedByAdminId = User.AdminId()
        });
        audit.Add("notification.sent", "student", model.StudentId, new { model.Title });
        await db.SaveChangesAsync();
        return Ok(new { sent = true });
    }

    [HttpGet("notifications")]
    [RequirePermission(Permissions.NotificationsSend)]
    public async Task<IActionResult> SentNotifications() =>
        Ok(await db.Notifications.AsNoTracking().OrderByDescending(n => n.CreatedAtUtc).Take(100)
            .Select(n => new { id = n.Id, studentId = n.StudentId, title = n.Title, body = n.Body, createdAtUtc = n.CreatedAtUtc }).ToListAsync());

    private async Task<long> NextUserIdAsync() => (await db.AuthUsers.MaxAsync(u => (long?)u.UserId) ?? 0) + 1;

    private static AuthUser NewUser(long userId, string username, string password)
    {
        var user = new AuthUser
        {
            Id = Guid.NewGuid(), UserId = userId, Username = username, SecurityStamp = Guid.NewGuid().ToString(),
            IsActive = true, CreatedAtUtc = DateTime.UtcNow
        };
        user.PasswordHash = Hasher.HashPassword(user, password);
        return user;
    }

    private static void Apply(AuthUser user, StudentRequest model)
    {
        user.FirstName = Null(model.FirstName);
        user.LastName = Null(model.LastName);
        user.Email = user.EmailId = Null(model.Email);
        user.PhoneNumber = Null(model.PhoneNumber);
        user.City = Null(model.City);
        user.IsActive = model.IsActive;
        if (model.IsActive) user.IsUserDeactivated = false;
    }

    private static string? Null(string? s) => string.IsNullOrWhiteSpace(s) ? null : s.Trim();
}
