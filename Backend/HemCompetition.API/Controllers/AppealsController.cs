using System.ComponentModel.DataAnnotations;
using HemCompetition.API.Data;
using HemCompetition.API.Domain;
using HemCompetition.API.Infrastructure;
using HemCompetition.API.Security;
using HemCompetition.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace HemCompetition.API.Controllers;

public sealed class AppealRequest
{
    [Required, StringLength(4000, MinimumLength = 20, ErrorMessage = "Please explain your request in at least 20 characters.")]
    public string Message { get; set; } = string.Empty;
}

/// <summary>
/// Appeals by restricted students. Reached with the appeal token issued at login, so filing an
/// appeal never gives access to the competition itself.
/// </summary>
[ApiController]
[Route("api/appeals")]
[Authorize(Policy = Policies.StudentOrAppeal)]
public class AppealsController(AppDbContext db, BanService bans, SettingsService settingsService) : ControllerBase
{
    [HttpGet("mine")]
    public async Task<IActionResult> Mine()
    {
        var studentId = User.StudentId();
        var settings = await settingsService.GetAsync();
        var ban = await bans.GetActiveAsync(studentId);
        var appeals = await db.Appeals.AsNoTracking().Where(a => a.StudentId == studentId)
            .OrderByDescending(a => a.SubmittedAtUtc).Take(20)
            .Select(a => new
            {
                id = a.Id, banId = a.BanId, message = a.Message, status = a.Status.ToString(),
                submittedAtUtc = a.SubmittedAtUtc, decidedAtUtc = a.DecidedAtUtc, decisionReason = a.DecisionReason
            }).ToListAsync();
        var used = ban == null ? 0 : appeals.Count(a => a.banId == ban.Id);
        return Ok(new
        {
            restricted = ban != null,
            ban = ban == null ? null : BanService.Describe(ban),
            appealsEnabled = settings.AppealsEnabled,
            canAppeal = ban != null && settings.AppealsEnabled && used < settings.MaxAppealsPerBan &&
                        appeals.All(a => a.banId != ban.Id || a.status != nameof(AppealStatus.Pending)),
            appealsRemaining = Math.Max(0, settings.MaxAppealsPerBan - used),
            supportContact = settings.SupportContact,
            appeals
        });
    }

    /// <summary>Files an appeal. It only creates a request: access stays restricted until an administrator decides.</summary>
    [HttpPost]
    [EnableRateLimiting("appeal")]
    public async Task<IActionResult> Submit([FromBody] AppealRequest model)
    {
        var studentId = User.StudentId();
        var settings = await settingsService.GetAsync();
        if (!settings.AppealsEnabled) throw ApiException.Forbidden("Appeals are not being accepted at the moment.");
        var ban = await bans.GetActiveAsync(studentId) ?? throw ApiException.Conflict("Your account is not restricted.", "NOT_RESTRICTED");
        if (await db.Appeals.AnyAsync(a => a.BanId == ban.Id && a.Status == AppealStatus.Pending))
            throw ApiException.Conflict("You already have a request under review.", "APPEAL_PENDING");
        if (await db.Appeals.CountAsync(a => a.BanId == ban.Id) >= settings.MaxAppealsPerBan)
            throw ApiException.Conflict("No further requests can be made for this restriction.", "APPEAL_LIMIT");

        var appeal = new Appeal
        {
            Id = Guid.NewGuid(), BanId = ban.Id, StudentId = studentId, Message = model.Message.Trim(),
            Status = AppealStatus.Pending, SubmittedAtUtc = DateTime.UtcNow
        };
        db.Appeals.Add(appeal);
        try
        {
            await db.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            throw ApiException.Conflict("You already have a request under review.", "APPEAL_PENDING");
        }
        return Ok(new { id = appeal.Id, status = appeal.Status.ToString(), submittedAtUtc = appeal.SubmittedAtUtc });
    }
}
