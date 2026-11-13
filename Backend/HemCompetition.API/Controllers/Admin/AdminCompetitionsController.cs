using System.ComponentModel.DataAnnotations;
using HemCompetition.API.Data;
using HemCompetition.API.Domain;
using HemCompetition.API.Infrastructure;
using HemCompetition.API.Security;
using HemCompetition.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HemCompetition.API.Controllers.Admin;

public sealed class CompetitionRequest
{
    [Required, StringLength(200)] public string CompetitionName { get; set; } = "";
    [StringLength(2000)] public string? Description { get; set; }
    public DateTime StartDateTimeUtc { get; set; }
    public DateTime EndDateTimeUtc { get; set; }
    public bool ShowResultsToStudents { get; set; }
    public bool ShowLeaderboard { get; set; }
    public bool ProctoringRequired { get; set; } = true;
}

[ApiController]
[Route("api/admin/competitions")]
[Authorize(Policy = Policies.Admin)]
public class AdminCompetitionsController(AppDbContext db, AuditService audit) : ControllerBase
{
    [HttpGet]
    [RequirePermission(Permissions.CompetitionsView)]
    public async Task<IActionResult> List()
    {
        var now = DateTime.UtcNow;
        var list = await db.CompetitionSettings.AsNoTracking().OrderByDescending(c => c.StartDateTimeUtc).Select(c => new
        {
            c.Id, c.CompetitionName, c.Description, c.StartDateTimeUtc, c.EndDateTimeUtc, c.IsActive, c.ShowResultsToStudents,
            c.ShowLeaderboard, c.ProctoringRequired,
            Participants = db.GameAttempts.Where(a => a.CompetitionId == c.Id).Select(a => a.StudentId)
                .Union(db.QuizAttempts.Where(a => a.CompetitionId == c.Id).Select(a => a.StudentId)).Count()
        }).ToListAsync();
        return Ok(list.Select(c => new
        {
            id = c.Id, competitionName = c.CompetitionName, description = c.Description, startDateTimeUtc = c.StartDateTimeUtc,
            endDateTimeUtc = c.EndDateTimeUtc, isActive = c.IsActive, showResultsToStudents = c.ShowResultsToStudents,
            showLeaderboard = c.ShowLeaderboard, proctoringRequired = c.ProctoringRequired, participants = c.Participants,
            phase = now < c.StartDateTimeUtc ? "Upcoming" : now <= c.EndDateTimeUtc ? "Live" : "Ended"
        }));
    }

    [HttpPost]
    [RequirePermission(Permissions.CompetitionsManage)]
    public async Task<IActionResult> Create([FromBody] CompetitionRequest model)
    {
        var c = new CompetitionSetting { Id = Guid.NewGuid(), CreatedAtUtc = DateTime.UtcNow, IsActive = false };
        Apply(c, model);
        db.CompetitionSettings.Add(c);
        audit.Add("competition.created", "competition", c.Id, new { c.CompetitionName });
        await db.SaveChangesAsync();
        return Ok(new { id = c.Id });
    }

    [HttpPut("{id:guid}")]
    [RequirePermission(Permissions.CompetitionsManage)]
    public async Task<IActionResult> Update(Guid id, [FromBody] CompetitionRequest model)
    {
        var c = await db.CompetitionSettings.FirstOrDefaultAsync(x => x.Id == id) ?? throw ApiException.NotFound("Competition not found.");
        Apply(c, model);
        c.UpdatedAtUtc = DateTime.UtcNow;
        audit.Add("competition.updated", "competition", id, new { model.StartDateTimeUtc, model.EndDateTimeUtc, model.ShowResultsToStudents, model.ShowLeaderboard });
        await db.SaveChangesAsync();
        return Ok(new { id });
    }

    /// <summary>Makes this the competition students see. Only one competition is active at a time.</summary>
    [HttpPost("{id:guid}/activate")]
    [RequirePermission(Permissions.CompetitionsManage)]
    public async Task<IActionResult> Activate(Guid id)
    {
        await using var tx = await db.Database.BeginTransactionAsync();
        if (!await db.CompetitionSettings.AnyAsync(x => x.Id == id)) throw ApiException.NotFound("Competition not found.");
        var now = DateTime.UtcNow;
        await db.CompetitionSettings.Where(x => x.IsActive && x.Id != id)
            .ExecuteUpdateAsync(u => u.SetProperty(x => x.IsActive, false).SetProperty(x => x.UpdatedAtUtc, now));
        await db.CompetitionSettings.Where(x => x.Id == id)
            .ExecuteUpdateAsync(u => u.SetProperty(x => x.IsActive, true).SetProperty(x => x.UpdatedAtUtc, now));
        audit.Add("competition.activated", "competition", id);
        await db.SaveChangesAsync();
        await tx.CommitAsync();
        return Ok(new { id, isActive = true });
    }

    [HttpPost("{id:guid}/deactivate")]
    [RequirePermission(Permissions.CompetitionsManage)]
    public async Task<IActionResult> Deactivate(Guid id)
    {
        var c = await db.CompetitionSettings.FirstOrDefaultAsync(x => x.Id == id) ?? throw ApiException.NotFound("Competition not found.");
        c.IsActive = false;
        c.UpdatedAtUtc = DateTime.UtcNow;
        audit.Add("competition.deactivated", "competition", id);
        await db.SaveChangesAsync();
        return Ok(new { id, isActive = false });
    }

    [HttpDelete("{id:guid}")]
    [RequirePermission(Permissions.CompetitionsManage)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var c = await db.CompetitionSettings.FirstOrDefaultAsync(x => x.Id == id) ?? throw ApiException.NotFound("Competition not found.");
        if (await db.GameAttempts.AnyAsync(a => a.CompetitionId == id) || await db.QuizAttempts.AnyAsync(a => a.CompetitionId == id))
            throw ApiException.Conflict("This competition has attempts and cannot be deleted. Deactivate it instead.");
        db.CompetitionSettings.Remove(c);
        audit.Add("competition.deleted", "competition", id, new { c.CompetitionName });
        await db.SaveChangesAsync();
        return Ok(new { id });
    }

    private static void Apply(CompetitionSetting c, CompetitionRequest model)
    {
        var start = DateTime.SpecifyKind(model.StartDateTimeUtc, DateTimeKind.Utc);
        var end = DateTime.SpecifyKind(model.EndDateTimeUtc, DateTimeKind.Utc);
        if (end <= start) throw ApiException.BadRequest("The end must be after the start.");
        c.CompetitionName = model.CompetitionName.Trim();
        c.Description = string.IsNullOrWhiteSpace(model.Description) ? null : model.Description.Trim();
        c.StartDateTimeUtc = start;
        c.EndDateTimeUtc = end;
        c.ShowResultsToStudents = model.ShowResultsToStudents;
        c.ShowLeaderboard = model.ShowLeaderboard;
        c.ProctoringRequired = model.ProctoringRequired;
    }
}
