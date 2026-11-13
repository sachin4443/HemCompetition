using HemCompetition.API.Data;
using HemCompetition.API.Domain;
using Microsoft.EntityFrameworkCore;

namespace HemCompetition.API.Services;

public sealed class LeaderboardRow
{
    public int Rank { get; set; }
    public long StudentId { get; set; }
    public string Name { get; set; } = "";
    public string? City { get; set; }
    public decimal QuizScore { get; set; }
    public decimal NumberScore { get; set; }
    public decimal WordScore { get; set; }
    public decimal TotalScore { get; set; }
    public int TimeTakenSeconds { get; set; }
    public int Attempts { get; set; }
    public bool Flagged { get; set; }
}

/// <summary>
/// Results are always computed from finished attempts, never stored separately, so they cannot
/// drift from the underlying data. Ranking: higher total first; ties go to the shorter total time.
/// </summary>
public sealed class LeaderboardService(AppDbContext db)
{
    public async Task<List<LeaderboardRow>> BuildAsync(Guid competitionId)
    {
        var games = await db.GameAttempts.AsNoTracking()
            .Where(a => a.CompetitionId == competitionId && a.Status != AttemptStatus.InProgress)
            .GroupBy(a => new { a.StudentId, a.Game.Category.Key })
            .Select(g => new { g.Key.StudentId, Category = g.Key.Key, Score = g.Sum(a => a.Score), Time = g.Sum(a => a.TimeTakenSeconds), Count = g.Count(), Flagged = g.Any(a => a.IsFlagged) })
            .ToListAsync();
        var quizzes = await db.QuizAttempts.AsNoTracking()
            .Where(a => a.CompetitionId == competitionId && a.Status != AttemptStatus.InProgress)
            .GroupBy(a => a.StudentId)
            .Select(g => new { StudentId = g.Key, Score = g.Sum(a => a.Score), Time = g.Sum(a => a.TimeTakenSeconds), Count = g.Count(), Flagged = g.Any(a => a.IsFlagged) })
            .ToListAsync();

        var rows = new Dictionary<long, LeaderboardRow>();
        LeaderboardRow Row(long id) => rows.TryGetValue(id, out var r) ? r : rows[id] = new LeaderboardRow { StudentId = id };
        foreach (var g in games)
        {
            var r = Row(g.StudentId);
            if (g.Category == "number") r.NumberScore += g.Score; else r.WordScore += g.Score;
            r.TimeTakenSeconds += g.Time; r.Attempts += g.Count; r.Flagged |= g.Flagged;
        }
        foreach (var q in quizzes)
        {
            var r = Row(q.StudentId);
            r.QuizScore += q.Score; r.TimeTakenSeconds += q.Time; r.Attempts += q.Count; r.Flagged |= q.Flagged;
        }

        var ordered = rows.Values.OrderByDescending(r => r.TotalScore = r.QuizScore + r.NumberScore + r.WordScore)
            .ThenBy(r => r.TimeTakenSeconds).ThenBy(r => r.StudentId).ToList();
        // Equal score and equal time share a rank.
        for (var i = 0; i < ordered.Count; i++)
            ordered[i].Rank = i > 0 && ordered[i].TotalScore == ordered[i - 1].TotalScore &&
                              ordered[i].TimeTakenSeconds == ordered[i - 1].TimeTakenSeconds ? ordered[i - 1].Rank : i + 1;
        return ordered;
    }

    public async Task FillNamesAsync(IReadOnlyCollection<LeaderboardRow> rows)
    {
        var ids = rows.Select(r => r.StudentId).ToList();
        var names = await db.AuthUsers.AsNoTracking().Where(u => ids.Contains(u.UserId))
            .Select(u => new { u.UserId, u.FirstName, u.LastName, u.Username, u.City }).ToDictionaryAsync(u => u.UserId);
        foreach (var r in rows)
            if (names.TryGetValue(r.StudentId, out var u))
            {
                var full = $"{u.FirstName} {u.LastName}".Trim();
                r.Name = full.Length > 0 ? full : u.Username;
                r.City = u.City;
            }
    }
}
