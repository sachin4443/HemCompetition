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

public sealed class AdminUserRequest
{
    [Required, StringLength(256, MinimumLength = 3)] public string Username { get; set; } = "";
    [StringLength(256)] public string? DisplayName { get; set; }
    [EmailAddress, StringLength(256)] public string? Email { get; set; }
    public int RoleId { get; set; }
    public bool IsActive { get; set; } = true;
    /// <summary>Required when creating; when editing, leave empty to keep the current password.</summary>
    [StringLength(200)] public string? Password { get; set; }
}

public sealed class RoleRequest
{
    [Required, StringLength(100, MinimumLength = 2)] public string Name { get; set; } = "";
    [StringLength(500)] public string? Description { get; set; }
    public List<string> Permissions { get; set; } = new();
}

[ApiController]
[Route("api/admin")]
[Authorize(Policy = Policies.Admin)]
public class AdminUsersController(AppDbContext db, AuditService audit, AdminAccessService access, TokenService tokens) : ControllerBase
{
    private static readonly PasswordHasher<AdminUser> Hasher = new();

    [HttpGet("admins")]
    [RequirePermission(Permissions.AdminsManage)]
    public async Task<IActionResult> Admins() =>
        Ok(await db.AdminUsers.AsNoTracking().OrderBy(a => a.Username).Select(a => new
        {
            id = a.Id, username = a.Username, displayName = a.DisplayName, email = a.Email, roleId = a.RoleId, role = a.Role!.Name,
            isActive = a.IsActive, mustChangePassword = a.MustChangePassword, lastLoginAtUtc = a.LastLoginAtUtc, createdAtUtc = a.CreatedAtUtc
        }).ToListAsync());

    [HttpPost("admins")]
    [RequirePermission(Permissions.AdminsManage)]
    public async Task<IActionResult> CreateAdmin([FromBody] AdminUserRequest model)
    {
        var username = model.Username.Trim();
        if (await db.AdminUsers.AnyAsync(a => a.Username == username)) throw ApiException.Conflict("That username is already in use.");
        if (!await db.AdminRoles.AnyAsync(r => r.Id == model.RoleId)) throw ApiException.BadRequest("Choose a role.");
        var problem = PasswordRules.Check(model.Password ?? "");
        if (problem != null) throw ApiException.BadRequest(problem);
        var admin = new AdminUser
        {
            Id = Guid.NewGuid(), Username = username, DisplayName = model.DisplayName?.Trim(), Email = model.Email?.Trim(),
            RoleId = model.RoleId, IsActive = model.IsActive, CreatedAtUtc = DateTime.UtcNow,
            // The creator knows this password, so the new admin must replace it.
            MustChangePassword = true
        };
        admin.PasswordHash = Hasher.HashPassword(admin, model.Password!);
        db.AdminUsers.Add(admin);
        audit.Add("admin.created", "admin", admin.Id, new { username, model.RoleId });
        await db.SaveChangesAsync();
        return Ok(new { id = admin.Id });
    }

    [HttpPut("admins/{id:guid}")]
    [RequirePermission(Permissions.AdminsManage)]
    public async Task<IActionResult> UpdateAdmin(Guid id, [FromBody] AdminUserRequest model)
    {
        var admin = await db.AdminUsers.Include(a => a.Role).FirstOrDefaultAsync(a => a.Id == id) ?? throw ApiException.NotFound();
        var username = model.Username.Trim();
        if (username != admin.Username && await db.AdminUsers.AnyAsync(a => a.Username == username)) throw ApiException.Conflict("That username is already in use.");
        var role = await db.AdminRoles.FirstOrDefaultAsync(r => r.Id == model.RoleId) ?? throw ApiException.BadRequest("Choose a role.");

        // Never leave the platform without an active Super Admin.
        var losesSuper = admin.Role?.Name == Permissions.SuperAdminRole && admin.IsActive &&
                         (role.Name != Permissions.SuperAdminRole || !model.IsActive);
        if (losesSuper && !await db.AdminUsers.AnyAsync(a => a.Id != id && a.IsActive && a.Role!.Name == Permissions.SuperAdminRole))
            throw ApiException.Conflict("At least one active Super Admin is required.");
        if (id == User.AdminId() && !model.IsActive) throw ApiException.Conflict("You cannot deactivate your own account.");

        admin.Username = username;
        admin.DisplayName = model.DisplayName?.Trim();
        admin.Email = model.Email?.Trim();
        admin.RoleId = role.Id;
        admin.IsActive = model.IsActive;
        admin.UpdatedAtUtc = DateTime.UtcNow;
        var passwordChanged = !string.IsNullOrEmpty(model.Password);
        if (passwordChanged)
        {
            var problem = PasswordRules.Check(model.Password!);
            if (problem != null) throw ApiException.BadRequest(problem);
            admin.PasswordHash = Hasher.HashPassword(admin, model.Password!);
            admin.MustChangePassword = id != User.AdminId();
            admin.AccessFailedCount = 0;
            admin.LockoutEndUtc = null;
        }
        audit.Add("admin.updated", "admin", id, new { model.RoleId, model.IsActive, passwordChanged });
        await db.SaveChangesAsync();
        if (!model.IsActive || passwordChanged) await tokens.RevokeAllForAdminAsync(id);
        access.Invalidate(id);
        return Ok(new { id });
    }

    // ----------------------------------------------------------------------------- roles

    [HttpGet("permissions")]
    [RequirePermission(Permissions.RolesManage)]
    public IActionResult PermissionCatalog() =>
        Ok(Permissions.Catalog.Select(p => new { key = p.Key, group = p.Group, label = p.Label }));

    [HttpGet("roles")]
    [RequirePermission(Permissions.AdminsManage)]
    public async Task<IActionResult> Roles()
    {
        var roles = await db.AdminRoles.AsNoTracking().Include(r => r.Permissions).OrderBy(r => r.Id).ToListAsync();
        var counts = await db.AdminUsers.Where(a => a.RoleId != null).GroupBy(a => a.RoleId!.Value).Select(g => new { g.Key, Count = g.Count() }).ToDictionaryAsync(x => x.Key, x => x.Count);
        return Ok(roles.Select(r => new
        {
            id = r.Id, name = r.Name, description = r.Description, isSystem = r.IsSystem,
            isSuperAdmin = r.Name == Permissions.SuperAdminRole,
            permissions = r.Name == Permissions.SuperAdminRole ? Permissions.All.OrderBy(p => p) : r.Permissions.Select(p => p.Permission).OrderBy(p => p),
            admins = counts.GetValueOrDefault(r.Id)
        }));
    }

    [HttpPost("roles")]
    [RequirePermission(Permissions.RolesManage)]
    public async Task<IActionResult> CreateRole([FromBody] RoleRequest model)
    {
        var name = model.Name.Trim();
        if (await db.AdminRoles.AnyAsync(r => r.Name == name)) throw ApiException.Conflict("A role with that name already exists.");
        var role = new AdminRole { Name = name, Description = model.Description?.Trim(), Permissions = Clean(model.Permissions) };
        db.AdminRoles.Add(role);
        audit.Add("role.created", "role", name, new { model.Permissions });
        await db.SaveChangesAsync();
        return Ok(new { id = role.Id });
    }

    [HttpPut("roles/{id:int}")]
    [RequirePermission(Permissions.RolesManage)]
    public async Task<IActionResult> UpdateRole(int id, [FromBody] RoleRequest model)
    {
        var role = await db.AdminRoles.Include(r => r.Permissions).FirstOrDefaultAsync(r => r.Id == id) ?? throw ApiException.NotFound();
        if (role.Name == Permissions.SuperAdminRole) throw ApiException.Conflict("The Super Admin role always has every permission and cannot be edited.");
        var name = model.Name.Trim();
        if (name != role.Name && (role.IsSystem || await db.AdminRoles.AnyAsync(r => r.Name == name)))
            throw ApiException.Conflict(role.IsSystem ? "Built-in roles cannot be renamed." : "A role with that name already exists.");
        role.Name = name;
        role.Description = model.Description?.Trim();
        db.AdminRolePermissions.RemoveRange(role.Permissions);
        role.Permissions = Clean(model.Permissions);
        audit.Add("role.updated", "role", id, new { model.Permissions });
        await db.SaveChangesAsync();
        return Ok(new { id });
    }

    [HttpDelete("roles/{id:int}")]
    [RequirePermission(Permissions.RolesManage)]
    public async Task<IActionResult> DeleteRole(int id)
    {
        var role = await db.AdminRoles.FirstOrDefaultAsync(r => r.Id == id) ?? throw ApiException.NotFound();
        if (role.IsSystem) throw ApiException.Conflict("Built-in roles cannot be deleted.");
        if (await db.AdminUsers.AnyAsync(a => a.RoleId == id)) throw ApiException.Conflict("Admins are still assigned to this role.");
        db.AdminRoles.Remove(role);
        audit.Add("role.deleted", "role", id, new { role.Name });
        await db.SaveChangesAsync();
        return Ok(new { id });
    }

    private static List<AdminRolePermission> Clean(IEnumerable<string> permissions) =>
        permissions.Where(Permissions.All.Contains).Distinct().Select(p => new AdminRolePermission { Permission = p }).ToList();
}

[ApiController]
[Route("api/admin")]
[Authorize(Policy = Policies.Admin)]
public class AdminSystemController(
    AppDbContext db, AuditService audit, SettingsService settingsService, CompetitionService competitions,
    AdminAccessService access, IApiMetricsService metrics) : ControllerBase
{
    [HttpGet("dashboard")]
    [RequirePermission(Permissions.DashboardView)]
    public async Task<IActionResult> Dashboard()
    {
        var now = DateTime.UtcNow;
        var live = now.AddSeconds(-150);
        var c = await competitions.GetActiveAsync();
        var cid = c?.Id;
        var me = await access.GetAsync(User.AdminId());
        return Ok(new
        {
            competition = c == null ? null : new
            {
                id = c.Id, name = c.CompetitionName, startDateTimeUtc = c.StartDateTimeUtc, endDateTimeUtc = c.EndDateTimeUtc,
                phase = CompetitionService.PhaseOf(c, now).ToString()
            },
            students = new
            {
                total = await db.AuthUsers.CountAsync(),
                active = await db.AuthUsers.CountAsync(u => u.IsActive && !u.IsUserDeactivated),
                online = await db.AuthSessions.Where(s => s.IsActive && s.LastHeartbeatUtc >= live).Select(s => s.UserId).Distinct().CountAsync(),
                participated = cid == null ? 0 : await db.GameAttempts.Where(a => a.CompetitionId == cid).Select(a => a.StudentId)
                    .Union(db.QuizAttempts.Where(a => a.CompetitionId == cid).Select(a => a.StudentId)).CountAsync(),
                restricted = await db.StudentBans.Where(b => b.LiftedAtUtc == null && (b.EndsAtUtc == null || b.EndsAtUtc > now)).Select(b => b.StudentId).Distinct().CountAsync()
            },
            attempts = new
            {
                gamesInProgress = await db.GameAttempts.CountAsync(a => a.CompetitionId == cid && a.Status == AttemptStatus.InProgress),
                gamesFinished = await db.GameAttempts.CountAsync(a => a.CompetitionId == cid && a.Status != AttemptStatus.InProgress),
                quizzesInProgress = await db.QuizAttempts.CountAsync(a => a.CompetitionId == cid && a.Status == AttemptStatus.InProgress),
                quizzesFinished = await db.QuizAttempts.CountAsync(a => a.CompetitionId == cid && a.Status != AttemptStatus.InProgress),
                flagged = await db.GameAttempts.CountAsync(a => a.CompetitionId == cid && a.IsFlagged) + await db.QuizAttempts.CountAsync(a => a.CompetitionId == cid && a.IsFlagged)
            },
            proctoring = new
            {
                monitoredNow = await db.ProctoringSessions.CountAsync(p => p.EndedAtUtc == null && p.LastSeenAtUtc >= now.AddMinutes(-5)),
                openViolations = await db.ViolationEvents.CountAsync(v => v.ReviewStatus == ReviewStatus.Open),
                openSerious = await db.ViolationEvents.CountAsync(v => v.ReviewStatus == ReviewStatus.Open && v.Severity != ViolationSeverity.Minor),
                violationsLastHour = await db.ViolationEvents.CountAsync(v => v.OccurredAtUtc >= now.AddHours(-1)),
                pendingAppeals = await db.Appeals.CountAsync(a => a.Status == AppealStatus.Pending)
            },
            content = new
            {
                publishedQuizzes = await db.Quizzes.CountAsync(q => q.IsPublished && !q.IsDeleted),
                questions = await db.Questions.CountAsync(q => !q.IsDeleted),
                words = await db.WordBank.CountAsync(w => w.IsActive)
            },
            // Request metrics are only for admins who may see diagnostics.
            metrics = me?.Has(Permissions.DiagnosticsView) == true ? metrics.GetSummary() : null,
            serverTimeUtc = now
        });
    }

    [HttpGet("settings")]
    [RequirePermission(Permissions.SettingsManage)]
    public async Task<IActionResult> GetSettings() => Ok(await settingsService.GetAsync());

    [HttpPut("settings")]
    [RequirePermission(Permissions.SettingsManage)]
    public async Task<IActionResult> SaveSettings([FromBody] PlatformSettings model)
    {
        await settingsService.SaveAsync(model, User.DisplayName());
        await audit.LogAsync("settings.updated", "settings", "platform", model);
        return Ok(await settingsService.GetAsync());
    }

    [HttpGet("audit-logs")]
    [RequirePermission(Permissions.AuditView)]
    public async Task<IActionResult> AuditLogs([FromQuery] PageQuery page, [FromQuery] string? action, [FromQuery] string? entityType,
        [FromQuery] DateTime? from, [FromQuery] DateTime? to)
    {
        var q = db.AuditLogs.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(action)) q = q.Where(a => a.Action.StartsWith(action));
        if (!string.IsNullOrWhiteSpace(entityType)) q = q.Where(a => a.EntityType == entityType);
        if (from != null) q = q.Where(a => a.CreatedAtUtc >= DateTime.SpecifyKind(from.Value, DateTimeKind.Utc));
        if (to != null) q = q.Where(a => a.CreatedAtUtc <= DateTime.SpecifyKind(to.Value, DateTimeKind.Utc));
        if (!string.IsNullOrWhiteSpace(page.Search))
        {
            var like = $"%{page.Search.Trim()}%";
            q = q.Where(a => EF.Functions.ILike(a.ActorName!, like) || EF.Functions.ILike(a.Action, like) || EF.Functions.ILike(a.EntityId!, like));
        }
        var total = await q.CountAsync();
        var items = await q.OrderByDescending(a => a.Id).Skip(page.Skip).Take(page.PageSize).Select(a => new
        {
            id = a.Id, createdAtUtc = a.CreatedAtUtc, actorType = a.ActorType, actorName = a.ActorName, action = a.Action,
            entityType = a.EntityType, entityId = a.EntityId, details = a.Details, ipAddress = a.IpAddress
        }).ToListAsync();
        return Ok(new PagedResult<object>(items, total, page.Page, page.PageSize));
    }
}
