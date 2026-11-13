using HemCompetition.API.Data;
using HemCompetition.API.Domain;
using HemCompetition.API.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace HemCompetition.API.Services;

/// <summary>One live session per student, kept alive by a heartbeat.</summary>
public sealed class SessionService(AppDbContext db, IOptions<HeartbeatOptions> options)
{
    private readonly HeartbeatOptions _options = options.Value;

    private DateTime StaleBefore => DateTime.UtcNow.AddSeconds(-_options.HeartbeatTimeoutSeconds);

    /// <summary>A session that is active and has sent a heartbeat recently. Stale sessions are closed.</summary>
    public async Task<AuthSession?> GetLiveAsync(long userId)
    {
        var sessions = await db.AuthSessions.Where(x => x.UserId == userId && x.IsActive).ToListAsync();
        AuthSession? live = null;
        foreach (var s in sessions)
        {
            if (s.LastHeartbeatUtc != null && s.LastHeartbeatUtc >= StaleBefore) live = s;
            else Close(s);
        }
        if (db.ChangeTracker.HasChanges()) await db.SaveChangesAsync();
        return live;
    }

    /// <summary>Adds a new session and closes any others. The caller saves.</summary>
    public async Task<AuthSession> CreateAsync(long userId, string? ip, string? userAgent)
    {
        foreach (var s in await db.AuthSessions.Where(x => x.UserId == userId && x.IsActive).ToListAsync()) Close(s);
        var now = DateTime.UtcNow;
        var session = new AuthSession
        {
            Id = Guid.NewGuid(), UserId = userId, SessionId = Guid.NewGuid(), CreatedAtUtc = now,
            LastActivityUtc = now, LastHeartbeatUtc = now, IsActive = true, IpAddress = ip,
            UserAgent = userAgent?.Length > 1000 ? userAgent[..1000] : userAgent
        };
        db.AuthSessions.Add(session);
        return session;
    }

    /// <summary>Read-only check used on every student request.</summary>
    public Task<bool> IsValidAsync(long userId, Guid sessionId)
    {
        var stale = StaleBefore;
        return db.AuthSessions.AsNoTracking().AnyAsync(x =>
            x.UserId == userId && x.SessionId == sessionId && x.IsActive && x.LastHeartbeatUtc >= stale);
    }

    public async Task<bool> HeartbeatAsync(long userId, Guid sessionId)
    {
        var now = DateTime.UtcNow;
        var stale = StaleBefore;
        var updated = await db.AuthSessions
            .Where(x => x.UserId == userId && x.SessionId == sessionId && x.IsActive && x.LastHeartbeatUtc >= stale)
            .ExecuteUpdateAsync(u => u.SetProperty(x => x.LastHeartbeatUtc, now).SetProperty(x => x.LastActivityUtc, now));
        return updated > 0;
    }

    public async Task TerminateAsync(long userId)
    {
        var now = DateTime.UtcNow;
        await db.AuthSessions.Where(x => x.UserId == userId && x.IsActive)
            .ExecuteUpdateAsync(u => u.SetProperty(x => x.IsActive, false)
                .SetProperty(x => x.TerminatedAtUtc, now).SetProperty(x => x.LastActivityUtc, now));
        await db.RefreshTokens.Where(t => t.StudentId == userId && t.RevokedAtUtc == null)
            .ExecuteUpdateAsync(u => u.SetProperty(t => t.RevokedAtUtc, now));
    }

    private static void Close(AuthSession s)
    {
        s.IsActive = false;
        s.TerminatedAtUtc = DateTime.UtcNow;
        s.LastActivityUtc = DateTime.UtcNow;
    }
}
