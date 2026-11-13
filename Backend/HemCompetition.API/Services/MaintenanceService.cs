using HemCompetition.API.Data;
using HemCompetition.API.Domain;
using HemCompetition.API.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace HemCompetition.API.Services;

/// <summary>
/// Housekeeping that must happen even when nobody is making requests: closing timed-out quizzes
/// and games, deleting evidence past its retention date, and clearing dead tokens and sessions.
/// </summary>
public sealed class MaintenanceService(IServiceScopeFactory scopes, ILogger<MaintenanceService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var tick = 0;
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(30));
        while (await timer.WaitForNextTickAsync(stoppingToken).ConfigureAwait(false))
        {
            try
            {
                using var scope = scopes.CreateScope();
                await RunAsync(scope.ServiceProvider, hourly: tick++ % 120 == 0, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested) { }
            catch (Exception ex)
            {
                logger.LogError(ex, "Maintenance run failed.");
            }
        }
    }

    public static async Task RunAsync(IServiceProvider services, bool hourly, CancellationToken ct = default)
    {
        var db = services.GetRequiredService<AppDbContext>();
        var now = DateTime.UtcNow;

        await services.GetRequiredService<QuizAttemptService>().SweepExpiredAsync();

        var games = await db.GameAttempts.Include(a => a.Rounds)
            .Where(a => a.Status == AttemptStatus.InProgress && a.DeadlineUtc < now).Take(200).ToListAsync(ct);
        foreach (var a in games) GameAttemptService.Finish(a, AttemptStatus.Expired);
        if (games.Count > 0) await db.SaveChangesAsync(ct);

        if (!hourly) return;

        // Evidence retention: remove the file, keep the metadata row as a record that it existed.
        var storage = services.GetRequiredService<IFileStorage>();
        var expired = await db.EvidenceFiles.Where(e => e.DeletedAtUtc == null && e.RetainUntilUtc < now).Take(500).ToListAsync(ct);
        foreach (var e in expired)
        {
            await storage.DeleteAsync(e.StorageKey);
            e.DeletedAtUtc = now;
            e.DeletedBy = "retention policy";
        }
        if (expired.Count > 0) await db.SaveChangesAsync(ct);

        await db.RefreshTokens.Where(t => t.ExpiresAtUtc < now.AddDays(-1)).ExecuteDeleteAsync(ct);
        await db.ProctoringSessions.Where(p => p.EndedAtUtc == null && p.LastSeenAtUtc < now.AddHours(-2))
            .ExecuteUpdateAsync(u => u.SetProperty(p => p.EndedAtUtc, now), ct);
    }
}
