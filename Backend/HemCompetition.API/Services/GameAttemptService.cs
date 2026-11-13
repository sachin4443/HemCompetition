using System.Text.Json;
using System.Text.Json.Nodes;
using HemCompetition.API.Data;
using HemCompetition.API.Domain;
using HemCompetition.API.Games;
using HemCompetition.API.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace HemCompetition.API.Services;

public sealed record GameSubmitResult(bool Correct, string? Message, object Attempt);

/// <summary>
/// The single attempt pipeline shared by every generated game: start, present a round, validate a
/// submission, move on, finish, score. Game-specific behaviour lives only in the engines.
/// </summary>
public sealed class GameAttemptService(AppDbContext db, GameEngineRegistry engines, CompetitionService competitions)
{
    /// <summary>Starts the student's attempt, or returns the one that already exists.</summary>
    public async Task<GameAttempt> StartAsync(long studentId, string gameKey)
    {
        var competition = await competitions.RequireLiveAsync();
        var game = await db.Games.AsNoTracking().Include(g => g.Category)
                       .FirstOrDefaultAsync(g => g.Key == gameKey && g.IsActive && g.Category.IsActive)
                   ?? throw ApiException.NotFound("This game is not available.");

        var existing = await Find(competition.Id, studentId, game.Id);
        if (existing != null) return existing;

        var engine = engines.Get(game.EngineKey);
        var context = new GenerationContext
        {
            Rng = new Random(),
            Difficulty = game.Difficulty,
            Config = JsonNode.Parse(game.Config) as JsonObject ?? new JsonObject(),
            Words = engine.RequiresWords
                ? await db.WordBank.AsNoTracking().Where(w => w.IsActive)
                    .Select(w => new WordEntry(w.Word, w.Clue, w.Topic)).ToListAsync()
                : []
        };

        var now = DateTime.UtcNow;
        var deadline = competition.EndDateTimeUtc;
        if (game.TimeLimitSeconds is > 0 && now.AddSeconds(game.TimeLimitSeconds.Value) < deadline)
            deadline = now.AddSeconds(game.TimeLimitSeconds.Value);

        var attempt = new GameAttempt
        {
            Id = Guid.NewGuid(), CompetitionId = competition.Id, StudentId = studentId, GameId = game.Id,
            Status = AttemptStatus.InProgress, Difficulty = game.Difficulty, ConfigSnapshot = game.Config,
            TotalRounds = game.Rounds, CurrentRound = 1, MaxScore = game.Rounds * game.PointsPerRound,
            StartedAtUtc = now, DeadlineUtc = deadline
        };
        try
        {
            for (var i = 1; i <= game.Rounds; i++)
            {
                var round = engine.Generate(context);
                attempt.Rounds.Add(new GameAttemptRound
                {
                    RoundNo = i, Puzzle = round.Puzzle.ToJsonString(), Solution = round.Solution.ToJsonString(),
                    Status = RoundStatus.Pending, PresentedAtUtc = i == 1 ? now : null
                });
            }
        }
        catch (GameUnavailableException ex)
        {
            throw ApiException.Conflict(ex.Message, "GAME_UNAVAILABLE");
        }

        db.GameAttempts.Add(attempt);
        try
        {
            await db.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            // Two starts raced (double click, second tab); the unique index kept exactly one.
            db.ChangeTracker.Clear();
            var raced = await Find(competition.Id, studentId, game.Id);
            if (raced == null) throw;
            return raced;
        }
        attempt.Game = game;
        return attempt;
    }

    private Task<GameAttempt?> Find(Guid competitionId, long studentId, int gameId) =>
        db.GameAttempts.Include(a => a.Game).Include(a => a.Rounds)
            .FirstOrDefaultAsync(a => a.CompetitionId == competitionId && a.StudentId == studentId && a.GameId == gameId);

    /// <summary>Loads an attempt the student owns, locking the row so concurrent submissions queue up.</summary>
    private async Task<GameAttempt> LoadOwnedAsync(long studentId, Guid attemptId, bool forUpdate)
    {
        if (forUpdate)
            await db.Database.ExecuteSqlInterpolatedAsync($"SELECT 1 FROM game_attempts WHERE id = {attemptId} FOR UPDATE");
        var attempt = await db.GameAttempts.Include(a => a.Game).Include(a => a.Rounds)
            .FirstOrDefaultAsync(a => a.Id == attemptId && a.StudentId == studentId);
        return attempt ?? throw ApiException.NotFound("Attempt not found.");
    }

    public async Task<object> GetAsync(long studentId, Guid attemptId)
    {
        var attempt = await LoadOwnedAsync(studentId, attemptId, forUpdate: false);
        if (ExpireIfDue(attempt) | MarkPresented(attempt)) await db.SaveChangesAsync();
        return View(attempt);
    }

    public async Task<GameSubmitResult> SubmitAsync(long studentId, Guid attemptId, int roundNo, JsonElement submission)
    {
        await using var tx = await db.Database.BeginTransactionAsync();
        var attempt = await LoadOwnedAsync(studentId, attemptId, forUpdate: true);
        var round = RequireCurrentRound(attempt, roundNo);

        var engine = engines.Get(attempt.Game.EngineKey);
        ValidationResult result;
        try
        {
            result = engine.Validate(JsonNode.Parse(round.Puzzle)!.AsObject(), JsonNode.Parse(round.Solution)!.AsObject(), submission);
        }
        catch (Exception ex) when (ex is InvalidOperationException or FormatException or KeyNotFoundException or NullReferenceException)
        {
            // A malformed submission is simply a wrong answer.
            result = ValidationResult.Wrong("That answer could not be read.");
        }

        var raw = submission.GetRawText();
        round.Submission = raw.Length <= 8000 ? raw : null;
        if (result.IsCorrect)
        {
            CloseRound(attempt, round, RoundStatus.Correct);
            Advance(attempt);
        }
        else
        {
            round.WrongSubmissions++;
        }
        await db.SaveChangesAsync();
        await tx.CommitAsync();
        return new GameSubmitResult(result.IsCorrect, result.Message, View(attempt));
    }

    /// <summary>"Next": gives up the current round and moves on. There is no way back.</summary>
    public async Task<object> SkipRoundAsync(long studentId, Guid attemptId, int roundNo)
    {
        await using var tx = await db.Database.BeginTransactionAsync();
        var attempt = await LoadOwnedAsync(studentId, attemptId, forUpdate: true);
        var round = RequireCurrentRound(attempt, roundNo);
        CloseRound(attempt, round, RoundStatus.Skipped);
        Advance(attempt);
        await db.SaveChangesAsync();
        await tx.CommitAsync();
        return View(attempt);
    }

    /// <summary>"Skip": leaves the game for good; unanswered rounds score nothing.</summary>
    public async Task<object> QuitAsync(long studentId, Guid attemptId)
    {
        await using var tx = await db.Database.BeginTransactionAsync();
        var attempt = await LoadOwnedAsync(studentId, attemptId, forUpdate: true);
        if (attempt.Status == AttemptStatus.InProgress && !ExpireIfDue(attempt)) Finish(attempt, AttemptStatus.Quit);
        await db.SaveChangesAsync();
        await tx.CommitAsync();
        return View(attempt);
    }

    /// <summary>Ends any running attempts for a student (used by the proctoring policy). The caller saves.</summary>
    public async Task<List<GameAttempt>> TerminateActiveAsync(long studentId)
    {
        var active = await db.GameAttempts.Include(a => a.Rounds)
            .Where(a => a.StudentId == studentId && a.Status == AttemptStatus.InProgress).ToListAsync();
        foreach (var a in active)
        {
            a.IsFlagged = true;
            Finish(a, AttemptStatus.Terminated);
        }
        return active;
    }

    private GameAttemptRound RequireCurrentRound(GameAttempt attempt, int roundNo)
    {
        if (ExpireIfDue(attempt))
        {
            db.SaveChanges();
            db.Database.CurrentTransaction?.Commit();
            throw ApiException.Conflict("Time is up for this game.", "ATTEMPT_EXPIRED");
        }
        if (attempt.Status != AttemptStatus.InProgress)
            throw ApiException.Conflict("This game is already finished.", "ATTEMPT_FINISHED");
        if (roundNo != attempt.CurrentRound)
            throw ApiException.Conflict("That round is no longer open.", "ROUND_CLOSED");
        return attempt.Rounds.First(r => r.RoundNo == roundNo);
    }

    private static void CloseRound(GameAttempt attempt, GameAttemptRound round, RoundStatus status)
    {
        var now = DateTime.UtcNow;
        round.Status = status;
        round.AnsweredAtUtc = now;
        round.TimeTakenSeconds = (int)Math.Max(0, (now - (round.PresentedAtUtc ?? attempt.StartedAtUtc)).TotalSeconds);
        round.Score = ScoreCalculator.RoundScore(attempt, status);
    }

    private static void Advance(GameAttempt attempt)
    {
        if (attempt.CurrentRound >= attempt.TotalRounds)
        {
            Finish(attempt, AttemptStatus.Completed);
            return;
        }
        attempt.CurrentRound++;
        attempt.Rounds.First(r => r.RoundNo == attempt.CurrentRound).PresentedAtUtc = DateTime.UtcNow;
        ScoreCalculator.Apply(attempt);
    }

    private static bool MarkPresented(GameAttempt attempt)
    {
        if (attempt.Status != AttemptStatus.InProgress) return false;
        var round = attempt.Rounds.First(r => r.RoundNo == attempt.CurrentRound);
        if (round.PresentedAtUtc != null) return false;
        round.PresentedAtUtc = DateTime.UtcNow;
        return true;
    }

    private static bool ExpireIfDue(GameAttempt attempt)
    {
        if (attempt.Status != AttemptStatus.InProgress || attempt.DeadlineUtc == null || DateTime.UtcNow <= attempt.DeadlineUtc) return false;
        Finish(attempt, AttemptStatus.Expired);
        return true;
    }

    public static void Finish(GameAttempt attempt, AttemptStatus status)
    {
        var now = DateTime.UtcNow;
        var end = status == AttemptStatus.Expired && attempt.DeadlineUtc != null ? attempt.DeadlineUtc.Value : now;
        foreach (var r in attempt.Rounds.Where(r => r.Status == RoundStatus.Pending))
        {
            r.Status = RoundStatus.Skipped;
            // Only the round that was on screen consumed time.
            if (r.RoundNo == attempt.CurrentRound && r.PresentedAtUtc != null)
                r.TimeTakenSeconds = (int)Math.Max(0, (end - r.PresentedAtUtc.Value).TotalSeconds);
        }
        attempt.Status = status;
        attempt.CompletedAtUtc = end;
        ScoreCalculator.Apply(attempt);
    }

    /// <summary>What a student may see about an attempt. The stored solution is never included.</summary>
    public static object View(GameAttempt a)
    {
        var current = a.Status == AttemptStatus.InProgress ? a.Rounds.First(r => r.RoundNo == a.CurrentRound) : null;
        return new
        {
            id = a.Id,
            gameKey = a.Game.Key,
            gameName = a.Game.Name,
            engine = a.Game.EngineKey,
            status = a.Status.ToString(),
            totalRounds = a.TotalRounds,
            currentRound = a.CurrentRound,
            correctCount = a.CorrectCount,
            startedAtUtc = a.StartedAtUtc,
            deadlineUtc = a.DeadlineUtc,
            serverTimeUtc = DateTime.UtcNow,
            round = current == null ? null : new
            {
                roundNo = current.RoundNo,
                puzzle = JsonNode.Parse(current.Puzzle),
                presentedAtUtc = current.PresentedAtUtc,
                wrongSubmissions = current.WrongSubmissions
            }
        };
    }
}

/// <summary>Scoring rules for generated games, in one place.</summary>
public static class ScoreCalculator
{
    public static decimal RoundScore(GameAttempt attempt, RoundStatus status) =>
        status == RoundStatus.Correct && attempt.TotalRounds > 0 ? attempt.MaxScore / attempt.TotalRounds : 0;

    public static void Apply(GameAttempt attempt)
    {
        attempt.CorrectCount = attempt.Rounds.Count(r => r.Status == RoundStatus.Correct);
        attempt.Score = attempt.Rounds.Sum(r => r.Score);
        attempt.TimeTakenSeconds = attempt.Rounds.Sum(r => r.TimeTakenSeconds);
    }
}
