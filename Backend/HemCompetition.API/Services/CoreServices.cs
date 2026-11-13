using System.Text.Json;
using HemCompetition.API.Data;
using HemCompetition.API.Domain;
using HemCompetition.API.Infrastructure;
using HemCompetition.API.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace HemCompetition.API.Services;

/// <summary>Settings an administrator can change at runtime (Security Settings and System Settings screens).</summary>
public sealed class PlatformSettings
{
    // --- sign-in ---
    public int MaxFailedLogins { get; set; } = 5;
    public int LockoutMinutes { get; set; } = 15;

    // --- proctoring ---
    /// <summary>Changing this makes every student accept the terms again.</summary>
    public string ConsentVersion { get; set; } = "2.0";
    public bool CameraRequired { get; set; } = true;
    public bool FullscreenRequired { get; set; } = true;
    public int NoFaceSeconds { get; set; } = 10;
    public int MultipleFacesSeconds { get; set; } = 10;
    public bool EvidenceCaptureEnabled { get; set; } = true;
    public bool VideoEvidenceEnabled { get; set; } = true;
    public int VideoClipSeconds { get; set; } = 6;
    public int EvidenceRetentionDays { get; set; } = 90;
    public int MaxEvidenceImageKb { get; set; } = 2048;
    public int MaxEvidenceVideoKb { get; set; } = 15360;

    // --- appeals ---
    public bool AppealsEnabled { get; set; } = true;
    public int MaxAppealsPerBan { get; set; } = 2;

    // --- general ---
    public string SupportContact { get; set; } = "";
    public int LeaderboardSize { get; set; } = 50;
}

public sealed class SettingsService(AppDbContext db, IMemoryCache cache)
{
    private const string Key = "platform";
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    public async Task<PlatformSettings> GetAsync()
    {
        if (cache.TryGetValue(Key, out PlatformSettings? cached) && cached != null) return cached;
        var row = await db.SystemSettings.AsNoTracking().FirstOrDefaultAsync(x => x.Key == Key);
        var settings = row == null ? new PlatformSettings() : JsonSerializer.Deserialize<PlatformSettings>(row.Value, Json) ?? new();
        cache.Set(Key, settings, TimeSpan.FromSeconds(30));
        return settings;
    }

    public async Task SaveAsync(PlatformSettings settings, string updatedBy)
    {
        settings.MaxFailedLogins = Math.Clamp(settings.MaxFailedLogins, 3, 20);
        settings.LockoutMinutes = Math.Clamp(settings.LockoutMinutes, 1, 1440);
        settings.NoFaceSeconds = Math.Clamp(settings.NoFaceSeconds, 3, 120);
        settings.MultipleFacesSeconds = Math.Clamp(settings.MultipleFacesSeconds, 3, 120);
        settings.VideoClipSeconds = Math.Clamp(settings.VideoClipSeconds, 2, 15);
        settings.EvidenceRetentionDays = Math.Clamp(settings.EvidenceRetentionDays, 1, 730);
        settings.MaxEvidenceImageKb = Math.Clamp(settings.MaxEvidenceImageKb, 100, 10240);
        settings.MaxEvidenceVideoKb = Math.Clamp(settings.MaxEvidenceVideoKb, 500, 51200);
        settings.MaxAppealsPerBan = Math.Clamp(settings.MaxAppealsPerBan, 1, 10);
        settings.LeaderboardSize = Math.Clamp(settings.LeaderboardSize, 5, 500);
        if (string.IsNullOrWhiteSpace(settings.ConsentVersion)) settings.ConsentVersion = "2.0";

        var row = await db.SystemSettings.FirstOrDefaultAsync(x => x.Key == Key);
        if (row == null) db.SystemSettings.Add(row = new SystemSetting { Key = Key });
        row.Value = JsonSerializer.Serialize(settings, Json);
        row.UpdatedAtUtc = DateTime.UtcNow;
        row.UpdatedBy = updatedBy;
        await db.SaveChangesAsync();
        cache.Remove(Key);
    }
}

/// <summary>Writes the audit trail. Every state-changing admin action goes through here.</summary>
public sealed class AuditService(AppDbContext db, IHttpContextAccessor http)
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    /// <summary>Adds an entry to the current unit of work; the caller's SaveChanges persists it atomically with the change.</summary>
    public void Add(string action, string? entityType = null, object? entityId = null, object? details = null)
    {
        var ctx = http.HttpContext;
        var user = ctx?.User;
        var isAdmin = user?.IsAdmin() == true;
        db.AuditLogs.Add(new AuditLog
        {
            ActorType = isAdmin ? "admin" : user?.Identity?.IsAuthenticated == true ? "student" : "system",
            ActorId = isAdmin ? user!.AdminId().ToString() : user?.FindFirst("UserId")?.Value,
            ActorName = user?.Identity?.IsAuthenticated == true ? user.DisplayName() : "system",
            Action = action,
            EntityType = entityType,
            EntityId = entityId?.ToString(),
            Details = details == null ? null : JsonSerializer.Serialize(details, Json),
            IpAddress = ctx?.Connection.RemoteIpAddress?.ToString(),
            CreatedAtUtc = DateTime.UtcNow
        });
    }

    public async Task LogAsync(string action, string? entityType = null, object? entityId = null, object? details = null)
    {
        Add(action, entityType, entityId, details);
        await db.SaveChangesAsync();
    }
}

public sealed record AdminAccess(Guid Id, string Username, bool IsActive, bool MustChangePassword, string? RoleName, bool IsSuperAdmin, IReadOnlySet<string> Permissions)
{
    public bool Has(string permission) => IsSuperAdmin || Permissions.Contains(permission);
    public IEnumerable<string> Effective => IsSuperAdmin ? Security.Permissions.All : Permissions;
}

/// <summary>Resolves an admin's current role and permissions.</summary>
public sealed class AdminAccessService(AppDbContext db, IMemoryCache cache)
{
    private static string CacheKey(Guid id) => $"admin-access:{id}";

    public async Task<AdminAccess?> GetAsync(Guid adminId)
    {
        if (cache.TryGetValue(CacheKey(adminId), out AdminAccess? cached)) return cached;
        var admin = await db.AdminUsers.AsNoTracking().Include(a => a.Role).ThenInclude(r => r!.Permissions)
            .FirstOrDefaultAsync(a => a.Id == adminId);
        var access = admin == null ? null : new AdminAccess(
            admin.Id, admin.Username, admin.IsActive, admin.MustChangePassword, admin.Role?.Name,
            admin.Role?.Name == Permissions.SuperAdminRole,
            admin.Role?.Permissions.Select(p => p.Permission).ToHashSet() ?? new HashSet<string>());
        cache.Set(CacheKey(adminId), access, TimeSpan.FromSeconds(15));
        return access;
    }

    public void Invalidate(Guid adminId) => cache.Remove(CacheKey(adminId));

    /// <summary>Role changes affect many admins; their entries simply expire (15 s).</summary>
    public void InvalidateAll() { }
}

public enum CompetitionPhase { None, NotStarted, Live, Ended }

public sealed class CompetitionService(AppDbContext db)
{
    public Task<CompetitionSetting?> GetActiveAsync() =>
        db.CompetitionSettings.AsNoTracking().Where(x => x.IsActive)
            .OrderByDescending(x => x.StartDateTimeUtc).FirstOrDefaultAsync();

    public static CompetitionPhase PhaseOf(CompetitionSetting? c, DateTime nowUtc) =>
        c == null ? CompetitionPhase.None
        : nowUtc < c.StartDateTimeUtc ? CompetitionPhase.NotStarted
        : nowUtc <= c.EndDateTimeUtc ? CompetitionPhase.Live
        : CompetitionPhase.Ended;

    /// <summary>The active competition, or an error when no competition is running right now.</summary>
    public async Task<CompetitionSetting> RequireLiveAsync()
    {
        var competition = await GetActiveAsync();
        return PhaseOf(competition, DateTime.UtcNow) switch
        {
            CompetitionPhase.Live => competition!,
            CompetitionPhase.NotStarted => throw ApiException.Conflict("The competition has not started yet.", "COMPETITION_NOT_STARTED"),
            CompetitionPhase.Ended => throw ApiException.Conflict("The competition has ended.", "COMPETITION_ENDED"),
            _ => throw ApiException.Conflict("There is no active competition.", "NO_COMPETITION")
        };
    }
}

public sealed class BanService(AppDbContext db)
{
    public async Task<StudentBan?> GetActiveAsync(long studentId)
    {
        var now = DateTime.UtcNow;
        return await db.StudentBans
            .Where(b => b.StudentId == studentId && b.LiftedAtUtc == null && b.StartsAtUtc <= now && (b.EndsAtUtc == null || b.EndsAtUtc > now))
            // A permanent ban outranks a suspension; otherwise the longest restriction wins.
            .OrderByDescending(b => b.Type == BanType.Ban).ThenByDescending(b => b.EndsAtUtc == null).ThenByDescending(b => b.EndsAtUtc)
            .FirstOrDefaultAsync();
    }

    /// <summary>Adds the ban and ends the student's sessions. The caller saves.</summary>
    public async Task<StudentBan> AddAsync(long studentId, BanType type, BanSource source, string reason, DateTime? endsAtUtc,
        Guid? competitionId = null, string? ruleCode = null, Guid? violationEventId = null, Guid? adminId = null)
    {
        var now = DateTime.UtcNow;
        var ban = new StudentBan
        {
            Id = Guid.NewGuid(), StudentId = studentId, CompetitionId = competitionId, Type = type, Source = source,
            Reason = reason, RuleCode = ruleCode, ViolationEventId = violationEventId, StartsAtUtc = now,
            EndsAtUtc = type == BanType.Ban ? null : endsAtUtc, CreatedByAdminId = adminId
        };
        db.StudentBans.Add(ban);

        foreach (var s in await db.AuthSessions.Where(x => x.UserId == studentId && x.IsActive).ToListAsync())
        {
            s.IsActive = false;
            s.TerminatedAtUtc = now;
        }
        await db.RefreshTokens.Where(t => t.StudentId == studentId && t.RevokedAtUtc == null)
            .ExecuteUpdateAsync(u => u.SetProperty(t => t.RevokedAtUtc, now));
        return ban;
    }

    public static object Describe(StudentBan ban) => new
    {
        id = ban.Id,
        type = ban.Type.ToString(),
        reason = ban.Reason,
        ruleCode = ban.RuleCode,
        startsAtUtc = ban.StartsAtUtc,
        endsAtUtc = ban.EndsAtUtc,
        permanent = ban.Type == BanType.Ban
    };
}
