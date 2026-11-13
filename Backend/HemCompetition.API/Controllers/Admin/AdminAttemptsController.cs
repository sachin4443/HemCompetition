using System.Text.Json;
using System.Text.Json.Nodes;
using HemCompetition.API.Data;
using HemCompetition.API.Domain;
using HemCompetition.API.Infrastructure;
using HemCompetition.API.Security;
using HemCompetition.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HemCompetition.API.Controllers.Admin;

public sealed class ResetRequest
{
    [System.ComponentModel.DataAnnotations.Required, System.ComponentModel.DataAnnotations.StringLength(1000, MinimumLength = 5)]
    public string Reason { get; set; } = "";
}

[ApiController]
[Route("api/admin")]
[Authorize(Policy = Policies.Admin)]
public class AdminAttemptsController(AppDbContext db, AuditService audit, LeaderboardService leaderboard, CompetitionService competitions) : ControllerBase
{
    private async Task<Guid?> ResolveCompetitionAsync(Guid? competitionId) =>
        competitionId ?? (await competitions.GetActiveAsync())?.Id
        ?? await db.CompetitionSettings.OrderByDescending(c => c.StartDateTimeUtc).Select(c => (Guid?)c.Id).FirstOrDefaultAsync();

    // ----------------------------------------------------------------------------- game attempts

    [HttpGet("game-attempts")]
    [RequirePermission(Permissions.AttemptsView)]
    public async Task<IActionResult> GameAttempts([FromQuery] PageQuery page, [FromQuery] Guid? competitionId, [FromQuery] int? gameId,
        [FromQuery] AttemptStatus? status, [FromQuery] bool? flagged)
    {
        var q = db.GameAttempts.AsNoTracking();
        if (competitionId != null) q = q.Where(a => a.CompetitionId == competitionId);
        if (gameId != null) q = q.Where(a => a.GameId == gameId);
        if (status != null) q = q.Where(a => a.Status == status);
        if (flagged != null) q = q.Where(a => a.IsFlagged == flagged);
        var joined = from a in q join u in db.AuthUsers.AsNoTracking() on a.StudentId equals u.UserId select new { a, u };
        if (!string.IsNullOrWhiteSpace(page.Search))
        {
            var s = page.Search.Trim();
            var like = $"%{s}%";
            joined = long.TryParse(s, out var id)
                ? joined.Where(x => x.u.UserId == id || EF.Functions.ILike(x.u.Username, like))
                : joined.Where(x => EF.Functions.ILike(x.u.Username, like) || EF.Functions.ILike(x.u.FirstName!, like) || EF.Functions.ILike(x.u.LastName!, like));
        }
        var total = await joined.CountAsync();
        var items = await joined.OrderByDescending(x => x.a.StartedAtUtc).Skip(page.Skip).Take(page.PageSize).Select(x => new
        {
            id = x.a.Id, studentId = x.a.StudentId, student = (x.u.FirstName + " " + x.u.LastName).Trim(), username = x.u.Username,
            game = x.a.Game.Name, section = x.a.Game.Category.Name, status = x.a.Status.ToString(), score = x.a.Score, maxScore = x.a.MaxScore,
            correct = x.a.CorrectCount, total = x.a.TotalRounds, timeTakenSeconds = x.a.TimeTakenSeconds,
            startedAtUtc = x.a.StartedAtUtc, completedAtUtc = x.a.CompletedAtUtc, isFlagged = x.a.IsFlagged
        }).ToListAsync();
        return Ok(new PagedResult<object>(items, total, page.Page, page.PageSize));
    }

    /// <summary>Full detail of one game attempt, including each generated puzzle, its solution and what was submitted.</summary>
    [HttpGet("game-attempts/{id:guid}")]
    [RequirePermission(Permissions.AttemptsView)]
    public async Task<IActionResult> GameAttempt(Guid id)
    {
        var a = await db.GameAttempts.AsNoTracking().Include(x => x.Game).Include(x => x.Rounds).FirstOrDefaultAsync(x => x.Id == id) ?? throw ApiException.NotFound();
        var u = await db.AuthUsers.AsNoTracking().FirstAsync(x => x.UserId == a.StudentId);
        return Ok(new
        {
            id = a.Id, studentId = a.StudentId, student = $"{u.FirstName} {u.LastName}".Trim(), username = u.Username,
            game = a.Game.Name, engine = a.Game.EngineKey, difficulty = a.Difficulty.ToString(), config = JsonNode.Parse(a.ConfigSnapshot),
            status = a.Status.ToString(), score = a.Score, maxScore = a.MaxScore, correct = a.CorrectCount, total = a.TotalRounds,
            timeTakenSeconds = a.TimeTakenSeconds, startedAtUtc = a.StartedAtUtc, completedAtUtc = a.CompletedAtUtc, isFlagged = a.IsFlagged,
            rounds = a.Rounds.OrderBy(r => r.RoundNo).Select(r => new
            {
                roundNo = r.RoundNo, status = r.Status.ToString(), score = r.Score, wrongSubmissions = r.WrongSubmissions,
                timeTakenSeconds = r.TimeTakenSeconds, presentedAtUtc = r.PresentedAtUtc, answeredAtUtc = r.AnsweredAtUtc,
                puzzle = JsonNode.Parse(r.Puzzle), solution = JsonNode.Parse(r.Solution),
                submission = r.Submission == null ? null : JsonNode.Parse(r.Submission)
            }),
            violations = await Violations(v => v.GameAttemptId == id)
        });
    }

    // ----------------------------------------------------------------------------- quiz attempts

    [HttpGet("quiz-attempts")]
    [RequirePermission(Permissions.AttemptsView)]
    public async Task<IActionResult> QuizAttempts([FromQuery] PageQuery page, [FromQuery] Guid? competitionId, [FromQuery] int? quizId,
        [FromQuery] AttemptStatus? status, [FromQuery] bool? flagged)
    {
        var q = db.QuizAttempts.AsNoTracking();
        if (competitionId != null) q = q.Where(a => a.CompetitionId == competitionId);
        if (quizId != null) q = q.Where(a => a.QuizId == quizId);
        if (status != null) q = q.Where(a => a.Status == status);
        if (flagged != null) q = q.Where(a => a.IsFlagged == flagged);
        var joined = from a in q join u in db.AuthUsers.AsNoTracking() on a.StudentId equals u.UserId select new { a, u };
        if (!string.IsNullOrWhiteSpace(page.Search))
        {
            var s = page.Search.Trim();
            var like = $"%{s}%";
            joined = long.TryParse(s, out var id)
                ? joined.Where(x => x.u.UserId == id || EF.Functions.ILike(x.u.Username, like))
                : joined.Where(x => EF.Functions.ILike(x.u.Username, like) || EF.Functions.ILike(x.u.FirstName!, like) || EF.Functions.ILike(x.u.LastName!, like));
        }
        var total = await joined.CountAsync();
        var items = await joined.OrderByDescending(x => x.a.StartedAtUtc).Skip(page.Skip).Take(page.PageSize).Select(x => new
        {
            id = x.a.Id, studentId = x.a.StudentId, student = (x.u.FirstName + " " + x.u.LastName).Trim(), username = x.u.Username,
            quiz = x.a.QuizTitle, quizVersion = x.a.QuizVersion, status = x.a.Status.ToString(), score = x.a.Score, maxScore = x.a.MaxScore,
            correct = x.a.CorrectCount, incorrect = x.a.IncorrectCount, unanswered = x.a.UnansweredCount,
            timeTakenSeconds = x.a.TimeTakenSeconds, startedAtUtc = x.a.StartedAtUtc, submittedAtUtc = x.a.SubmittedAtUtc, isFlagged = x.a.IsFlagged
        }).ToListAsync();
        return Ok(new PagedResult<object>(items, total, page.Page, page.PageSize));
    }

    [HttpGet("quiz-attempts/{id:guid}")]
    [RequirePermission(Permissions.AttemptsView)]
    public async Task<IActionResult> QuizAttempt(Guid id)
    {
        var a = await db.QuizAttempts.AsNoTracking().Include(x => x.Questions).FirstOrDefaultAsync(x => x.Id == id) ?? throw ApiException.NotFound();
        var u = await db.AuthUsers.AsNoTracking().FirstAsync(x => x.UserId == a.StudentId);
        return Ok(new
        {
            id = a.Id, studentId = a.StudentId, student = $"{u.FirstName} {u.LastName}".Trim(), username = u.Username,
            quiz = a.QuizTitle, quizVersion = a.QuizVersion, status = a.Status.ToString(), score = a.Score, maxScore = a.MaxScore,
            correct = a.CorrectCount, incorrect = a.IncorrectCount, unanswered = a.UnansweredCount, negativeMarks = a.NegativeMarks,
            timeTakenSeconds = a.TimeTakenSeconds, startedAtUtc = a.StartedAtUtc, expiresAtUtc = a.ExpiresAtUtc, submittedAtUtc = a.SubmittedAtUtc, isFlagged = a.IsFlagged,
            questions = a.Questions.OrderBy(q => q.OrderNo).Select(q =>
            {
                var s = JsonSerializer.Deserialize<QuestionSnapshot>(q.Snapshot, QuizAttemptService.Json)!;
                return new
                {
                    orderNo = q.OrderNo, questionId = q.QuestionId, text = s.Text, marks = s.Marks, explanation = s.Explanation,
                    options = s.Options.Select(o => new { id = o.Id, text = o.Text, isCorrect = o.Id == s.CorrectOptionId }),
                    selectedOptionId = q.SelectedOptionId, isCorrect = q.IsCorrect, awardedMarks = q.AwardedMarks, answeredAtUtc = q.AnsweredAtUtc
                };
            }),
            violations = await Violations(v => v.QuizAttemptId == id)
        });
    }

    /// <summary>
    /// Deletes an attempt so the student can take it again (for example after an upheld appeal).
    /// Scores are never edited directly; the only way to change a result is a fresh attempt.
    /// </summary>
    [HttpPost("game-attempts/{id:guid}/reset")]
    [RequirePermission(Permissions.AttemptsManage)]
    public async Task<IActionResult> ResetGameAttempt(Guid id, [FromBody] ResetRequest model)
    {
        var a = await db.GameAttempts.Include(x => x.Game).FirstOrDefaultAsync(x => x.Id == id) ?? throw ApiException.NotFound();
        db.GameAttempts.Remove(a);
        audit.Add("game_attempt.reset", "student", a.StudentId, new { attemptId = id, game = a.Game.Name, a.Score, status = a.Status.ToString(), model.Reason });
        await db.SaveChangesAsync();
        return Ok(new { id });
    }

    [HttpPost("quiz-attempts/{id:guid}/reset")]
    [RequirePermission(Permissions.AttemptsManage)]
    public async Task<IActionResult> ResetQuizAttempt(Guid id, [FromBody] ResetRequest model)
    {
        var a = await db.QuizAttempts.FirstOrDefaultAsync(x => x.Id == id) ?? throw ApiException.NotFound();
        db.QuizAttempts.Remove(a);
        audit.Add("quiz_attempt.reset", "student", a.StudentId, new { attemptId = id, quiz = a.QuizTitle, a.Score, status = a.Status.ToString(), model.Reason });
        await db.SaveChangesAsync();
        return Ok(new { id });
    }

    [HttpPost("{kind:regex(^(game|quiz)-attempts$)}/{id:guid}/flag")]
    [RequirePermission(Permissions.AttemptsManage)]
    public async Task<IActionResult> Flag(string kind, Guid id, [FromQuery] bool value)
    {
        var changed = kind == "game-attempts"
            ? await db.GameAttempts.Where(a => a.Id == id).ExecuteUpdateAsync(u => u.SetProperty(a => a.IsFlagged, value))
            : await db.QuizAttempts.Where(a => a.Id == id).ExecuteUpdateAsync(u => u.SetProperty(a => a.IsFlagged, value));
        if (changed == 0) throw ApiException.NotFound();
        await audit.LogAsync(value ? "attempt.flagged" : "attempt.unflagged", kind, id);
        return Ok(new { id, isFlagged = value });
    }

    // ----------------------------------------------------------------------------- results & reports

    [HttpGet("results/leaderboard")]
    [RequirePermission(Permissions.ResultsView)]
    public async Task<IActionResult> Leaderboard([FromQuery] PageQuery page, [FromQuery] Guid? competitionId)
    {
        var id = await ResolveCompetitionAsync(competitionId);
        if (id == null) return Ok(new PagedResult<LeaderboardRow>([], 0, 1, page.PageSize));
        var rows = await leaderboard.BuildAsync(id.Value);
        await leaderboard.FillNamesAsync(rows);
        if (!string.IsNullOrWhiteSpace(page.Search))
            rows = rows.Where(r => r.Name.Contains(page.Search.Trim(), StringComparison.OrdinalIgnoreCase) || r.StudentId.ToString() == page.Search.Trim()).ToList();
        return Ok(new PagedResult<LeaderboardRow>(rows.Skip(page.Skip).Take(page.PageSize).ToList(), rows.Count, page.Page, page.PageSize));
    }

    /// <summary>Participation and average scores per game and quiz.</summary>
    [HttpGet("results/summary")]
    [RequirePermission(Permissions.ResultsView)]
    public async Task<IActionResult> Summary([FromQuery] Guid? competitionId)
    {
        var id = await ResolveCompetitionAsync(competitionId);
        var games = await db.GameAttempts.AsNoTracking().Where(a => a.CompetitionId == id)
            .GroupBy(a => new { a.Game.Name, Section = a.Game.Category.Name })
            .Select(g => new
            {
                name = g.Key.Name, section = g.Key.Section, started = g.Count(),
                finished = g.Count(a => a.Status != AttemptStatus.InProgress),
                inProgress = g.Count(a => a.Status == AttemptStatus.InProgress),
                averageScore = g.Where(a => a.Status != AttemptStatus.InProgress).Average(a => (decimal?)a.Score) ?? 0,
                averageSeconds = g.Where(a => a.Status != AttemptStatus.InProgress).Average(a => (double?)a.TimeTakenSeconds) ?? 0,
                flagged = g.Count(a => a.IsFlagged)
            }).ToListAsync();
        var quizzes = await db.QuizAttempts.AsNoTracking().Where(a => a.CompetitionId == id)
            .GroupBy(a => a.QuizTitle)
            .Select(g => new
            {
                name = g.Key, section = "Mega Quiz", started = g.Count(),
                finished = g.Count(a => a.Status != AttemptStatus.InProgress),
                inProgress = g.Count(a => a.Status == AttemptStatus.InProgress),
                averageScore = g.Where(a => a.Status != AttemptStatus.InProgress).Average(a => (decimal?)a.Score) ?? 0,
                averageSeconds = g.Where(a => a.Status != AttemptStatus.InProgress).Average(a => (double?)a.TimeTakenSeconds) ?? 0,
                flagged = g.Count(a => a.IsFlagged)
            }).ToListAsync();
        return Ok(new { competitionId = id, items = games.Concat(quizzes).OrderBy(x => x.section).ThenBy(x => x.name) });
    }

    [HttpGet("reports/{report}")]
    [RequirePermission(Permissions.ReportsExport)]
    public async Task<IActionResult> Report(string report, [FromQuery] Guid? competitionId)
    {
        var id = await ResolveCompetitionAsync(competitionId);
        byte[] csv;
        switch (report)
        {
            case "leaderboard":
                var rows = id == null ? [] : await leaderboard.BuildAsync(id.Value);
                await leaderboard.FillNamesAsync(rows);
                csv = TabularFile.ToCsv(["rank", "student_id", "name", "city", "quiz_score", "number_game_score", "word_game_score", "total_score", "time_seconds", "attempts", "flagged"],
                    rows.Select(r => new object?[] { r.Rank, r.StudentId, r.Name, r.City, r.QuizScore, r.NumberScore, r.WordScore, r.TotalScore, r.TimeTakenSeconds, r.Attempts, r.Flagged }));
                break;
            case "game-attempts":
                var games = await (from a in db.GameAttempts.AsNoTracking().Where(a => a.CompetitionId == id)
                    join u in db.AuthUsers.AsNoTracking() on a.StudentId equals u.UserId
                    orderby a.StartedAtUtc
                    select new object?[] { a.StudentId, u.Username, u.FirstName, u.LastName, a.Game.Category.Name, a.Game.Name, a.Status.ToString(), a.Score, a.MaxScore,
                        a.CorrectCount, a.TotalRounds, a.TimeTakenSeconds, a.StartedAtUtc, a.CompletedAtUtc, a.IsFlagged }).ToListAsync();
                csv = TabularFile.ToCsv(["student_id", "username", "first_name", "last_name", "section", "game", "status", "score", "max_score", "correct", "rounds", "time_seconds", "started_utc", "completed_utc", "flagged"], games);
                break;
            case "quiz-attempts":
                var quizzes = await (from a in db.QuizAttempts.AsNoTracking().Where(a => a.CompetitionId == id)
                    join u in db.AuthUsers.AsNoTracking() on a.StudentId equals u.UserId
                    orderby a.StartedAtUtc
                    select new object?[] { a.StudentId, u.Username, u.FirstName, u.LastName, a.QuizTitle, a.QuizVersion, a.Status.ToString(), a.Score, a.MaxScore,
                        a.CorrectCount, a.IncorrectCount, a.UnansweredCount, a.TimeTakenSeconds, a.StartedAtUtc, a.SubmittedAtUtc, a.IsFlagged }).ToListAsync();
                csv = TabularFile.ToCsv(["student_id", "username", "first_name", "last_name", "quiz", "quiz_version", "status", "score", "max_score", "correct", "incorrect", "unanswered", "time_seconds", "started_utc", "submitted_utc", "flagged"], quizzes);
                break;
            case "violations":
                var violations = await (from v in db.ViolationEvents.AsNoTracking().Where(v => v.CompetitionId == id)
                    join u in db.AuthUsers.AsNoTracking() on v.StudentId equals u.UserId
                    orderby v.OccurredAtUtc
                    select new object?[] { v.OccurredAtUtc, v.StudentId, u.Username, u.FirstName, u.LastName, v.RuleCode, v.Severity.ToString(), v.OccurrenceNo,
                        v.ActionTaken.ToString(), v.ReviewStatus.ToString(), v.Message }).ToListAsync();
                csv = TabularFile.ToCsv(["occurred_utc", "student_id", "username", "first_name", "last_name", "rule", "severity", "occurrence", "action", "review", "message"], violations);
                break;
            case "participation":
                var started = db.GameAttempts.Where(a => a.CompetitionId == id).Select(a => a.StudentId)
                    .Union(db.QuizAttempts.Where(a => a.CompetitionId == id).Select(a => a.StudentId));
                var students = await db.AuthUsers.AsNoTracking().OrderBy(u => u.UserId)
                    .Select(u => new object?[] { u.UserId, u.Username, u.FirstName, u.LastName, u.City, u.LastLoginAtUtc, started.Contains(u.UserId) }).ToListAsync();
                csv = TabularFile.ToCsv(["student_id", "username", "first_name", "last_name", "city", "last_login_utc", "participated"], students);
                break;
            default:
                throw ApiException.NotFound("Unknown report.");
        }
        await audit.LogAsync("report.exported", "report", report, new { competitionId = id });
        return File(csv, "text/csv", $"{report}_{DateTime.UtcNow:yyyyMMdd_HHmm}.csv");
    }

    private Task<List<object>> Violations(System.Linq.Expressions.Expression<Func<ViolationEvent, bool>> filter) =>
        db.ViolationEvents.AsNoTracking().Where(filter).OrderBy(v => v.OccurredAtUtc).Select(v => (object)new
        {
            id = v.Id, ruleCode = v.RuleCode, severity = v.Severity.ToString(), message = v.Message, action = v.ActionTaken.ToString(),
            occurredAtUtc = v.OccurredAtUtc, reviewStatus = v.ReviewStatus.ToString(), evidenceCount = v.Evidence.Count(e => e.DeletedAtUtc == null)
        }).ToListAsync();
}
