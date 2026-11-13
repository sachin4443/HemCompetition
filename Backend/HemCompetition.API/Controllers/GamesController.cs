using System.Text.Json;
using HemCompetition.API.Data;
using HemCompetition.API.Domain;
using HemCompetition.API.Security;
using HemCompetition.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HemCompetition.API.Controllers;

public sealed class RoundRequest
{
    public int RoundNo { get; set; }
    public JsonElement Answer { get; set; }
}

[ApiController]
[Route("api/games")]
[Authorize(Policy = Policies.Student)]
public class GamesController(AppDbContext db, GameAttemptService attempts, CompetitionService competitions) : ControllerBase
{
    /// <summary>The generated-game sections with the student's status in each game.</summary>
    [HttpGet]
    public async Task<IActionResult> List()
    {
        var studentId = User.StudentId();
        var competition = await competitions.GetActiveAsync();
        var mine = competition == null
            ? []
            : await db.GameAttempts.AsNoTracking().Where(a => a.StudentId == studentId && a.CompetitionId == competition.Id)
                .Select(a => new { a.GameId, a.Id, a.Status, a.CurrentRound, a.TotalRounds }).ToListAsync();
        var categories = await db.GameCategories.AsNoTracking().Where(c => c.IsActive).OrderBy(c => c.SortOrder)
            .Select(c => new
            {
                c.Key, c.Name, c.Description,
                Games = c.Games.Where(g => g.IsActive).OrderBy(g => g.SortOrder)
                    .Select(g => new { g.Id, g.Key, g.Name, g.Description, g.Rounds, g.TimeLimitSeconds, Difficulty = g.Difficulty.ToString() }).ToList()
            }).ToListAsync();

        return Ok(categories.Select(c => new
        {
            key = c.Key, name = c.Name, description = c.Description,
            games = c.Games.Select(g =>
            {
                var a = mine.FirstOrDefault(m => m.GameId == g.Id);
                return new
                {
                    key = g.Key, name = g.Name, description = g.Description, rounds = g.Rounds,
                    timeLimitSeconds = g.TimeLimitSeconds, difficulty = g.Difficulty,
                    attemptId = a?.Id,
                    // not_started | in_progress | finished - drives the Start / Continue / Completed button
                    status = a == null ? "not_started" : a.Status == AttemptStatus.InProgress ? "in_progress" : "finished",
                    progress = a == null ? 0 : a.Status == AttemptStatus.InProgress ? a.CurrentRound - 1 : a.TotalRounds
                };
            })
        }));
    }

    [HttpPost("{gameKey}/start")]
    public async Task<IActionResult> Start(string gameKey) =>
        Ok(GameAttemptService.View(await attempts.StartAsync(User.StudentId(), gameKey)));

    [HttpGet("attempts/{attemptId:guid}")]
    public async Task<IActionResult> Get(Guid attemptId) => Ok(await attempts.GetAsync(User.StudentId(), attemptId));

    [HttpPost("attempts/{attemptId:guid}/submit")]
    public async Task<IActionResult> Submit(Guid attemptId, [FromBody] RoundRequest model)
    {
        var result = await attempts.SubmitAsync(User.StudentId(), attemptId, model.RoundNo, model.Answer);
        return Ok(new { correct = result.Correct, message = result.Message, attempt = result.Attempt });
    }

    [HttpPost("attempts/{attemptId:guid}/next")]
    public async Task<IActionResult> Next(Guid attemptId, [FromBody] RoundRequest model) =>
        Ok(await attempts.SkipRoundAsync(User.StudentId(), attemptId, model.RoundNo));

    [HttpPost("attempts/{attemptId:guid}/quit")]
    public async Task<IActionResult> Quit(Guid attemptId) => Ok(await attempts.QuitAsync(User.StudentId(), attemptId));
}
