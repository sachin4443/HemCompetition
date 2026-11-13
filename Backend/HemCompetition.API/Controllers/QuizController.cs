using System.Text.Json;
using HemCompetition.API.Data;
using HemCompetition.API.Domain;
using HemCompetition.API.Infrastructure;
using HemCompetition.API.Security;
using HemCompetition.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HemCompetition.API.Controllers;

public sealed class AnswerRequest
{
    public int OrderNo { get; set; }
    public int? OptionId { get; set; }
}

[ApiController]
[Route("api/quiz")]
[Authorize(Policy = Policies.Student)]
public class QuizController(AppDbContext db, QuizAttemptService quizzes, CompetitionService competitions, IFileStorage storage) : ControllerBase
{
    /// <summary>Published quizzes with the student's status in each.</summary>
    [HttpGet]
    public async Task<IActionResult> List()
    {
        var studentId = User.StudentId();
        var competition = await competitions.GetActiveAsync();
        var mine = competition == null
            ? []
            : await db.QuizAttempts.AsNoTracking().Where(a => a.StudentId == studentId && a.CompetitionId == competition.Id)
                .Select(a => new { a.QuizId, a.Id, a.Status }).ToListAsync();
        var list = await db.Quizzes.AsNoTracking().Where(q => q.IsPublished && !q.IsDeleted).OrderBy(q => q.SortOrder).ThenBy(q => q.Id)
            .Select(q => new { q.Id, q.Title, q.Description, q.DurationMinutes, q.QuestionCount, q.NegativeMarks, q.SelectionMode,
                Fixed = q.Questions.Count }).ToListAsync();
        return Ok(list.Select(q =>
        {
            var a = mine.FirstOrDefault(m => m.QuizId == q.Id);
            return new
            {
                id = q.Id, title = q.Title, description = q.Description, durationMinutes = q.DurationMinutes,
                questionCount = q.SelectionMode == QuizSelectionMode.Fixed && q.Fixed > 0 ? q.Fixed : q.QuestionCount,
                negativeMarks = q.NegativeMarks,
                attemptId = a?.Id,
                status = a == null ? "not_started" : a.Status == AttemptStatus.InProgress ? "in_progress" : "finished"
            };
        }));
    }

    /// <summary>Starts the quiz, or resumes it after a refresh or reconnect.</summary>
    [HttpPost("{quizId:int}/start")]
    public async Task<IActionResult> Start(int quizId) =>
        Ok(QuizAttemptService.PaperView(await quizzes.StartAsync(User.StudentId(), quizId)));

    [HttpGet("attempts/{attemptId:guid}")]
    public async Task<IActionResult> Get(Guid attemptId)
    {
        var attempt = await quizzes.LoadOwnedAsync(User.StudentId(), attemptId);
        if (await quizzes.SubmitIfExpiredAsync(attempt)) await db.SaveChangesAsync();
        return Ok(QuizAttemptService.PaperView(attempt));
    }

    [HttpPut("attempts/{attemptId:guid}/answer")]
    public async Task<IActionResult> Answer(Guid attemptId, [FromBody] AnswerRequest model)
    {
        await quizzes.SaveAnswerAsync(User.StudentId(), attemptId, model.OrderNo, model.OptionId);
        return Ok(new { saved = true });
    }

    [HttpPost("attempts/{attemptId:guid}/submit")]
    public async Task<IActionResult> Submit(Guid attemptId)
    {
        var attempt = await quizzes.SubmitAsync(User.StudentId(), attemptId);
        return Ok(await Result(attempt));
    }

    [HttpGet("attempts/{attemptId:guid}/result")]
    public async Task<IActionResult> GetResult(Guid attemptId)
    {
        var attempt = await quizzes.LoadOwnedAsync(User.StudentId(), attemptId);
        if (await quizzes.SubmitIfExpiredAsync(attempt)) await db.SaveChangesAsync();
        if (attempt.Status == AttemptStatus.InProgress) throw ApiException.Conflict("This quiz has not been submitted yet.");
        return Ok(await Result(attempt));
    }

    /// <summary>Image of a question in the student's own running attempt.</summary>
    [HttpGet("attempts/{attemptId:guid}/questions/{orderNo:int}/image")]
    public async Task<IActionResult> Image(Guid attemptId, int orderNo)
    {
        var studentId = User.StudentId();
        var snapshotJson = await db.QuizAttemptQuestions.AsNoTracking()
            .Where(q => q.AttemptId == attemptId && q.OrderNo == orderNo && q.Attempt.StudentId == studentId)
            .Select(q => q.Snapshot).FirstOrDefaultAsync();
        var snapshot = snapshotJson == null ? null : JsonSerializer.Deserialize<QuestionSnapshot>(snapshotJson, QuizAttemptService.Json);
        if (snapshot?.ImageKey == null) return NotFound();
        var stream = await storage.OpenAsync(snapshot.ImageKey);
        if (stream == null) return NotFound();
        Response.Headers.CacheControl = "private, max-age=3600";
        return File(stream, snapshot.ImageContentType ?? "application/octet-stream");
    }

    private async Task<object> Result(QuizAttempt attempt)
    {
        var competition = await db.CompetitionSettings.AsNoTracking().FirstAsync(c => c.Id == attempt.CompetitionId);
        return QuizAttemptService.ResultView(attempt, competition);
    }
}
