using HemCompetition.API.Data;
using HemCompetition.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HemCompetition.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompetitionSettingController : ControllerBase
    {
        private readonly AuthDbContext _context;

        public CompetitionSettingController(AuthDbContext context)
        {
            _context = context;
        }

        // =========================================================
        // PUBLIC
        // Login se pehle competition ka date/time check karne ke liye
        // =========================================================

        [AllowAnonymous]
        [HttpGet("active")]
        public async Task<IActionResult> GetActiveCompetition()
        {
            var setting = await _context.CompetitionSettings
                .AsNoTracking()
                .Where(x => x.IsActive)
                .OrderByDescending(x => x.StartDateTimeUtc)
                .FirstOrDefaultAsync();

            if (setting == null)
            {
                return NotFound(new
                {
                    message = "No active competition found."
                });
            }

            var now = DateTime.UtcNow;

            string status;

            if (now < setting.StartDateTimeUtc)
            {
                status = "NOT_STARTED";
            }
            else if (now >= setting.StartDateTimeUtc &&
                     now <= setting.EndDateTimeUtc)
            {
                status = "LIVE";
            }
            else
            {
                status = "ENDED";
            }

            return Ok(new
            {
                id = setting.Id,
                competitionName = setting.CompetitionName,

                startDateTimeUtc = setting.StartDateTimeUtc,
                endDateTimeUtc = setting.EndDateTimeUtc,

                serverTimeUtc = now,

                status = status
            });
        }


        // =========================================================
        // ADMIN - CREATE / UPDATE COMPETITION
        // Baad mein Admin Panel se use karenge
        // =========================================================

        [Authorize]
        [HttpPost("save")]
        public async Task<IActionResult> SaveCompetition(
            [FromBody] CompetitionSetting model)
        {
            if (string.IsNullOrWhiteSpace(model.CompetitionName))
            {
                return BadRequest(new
                {
                    message = "Competition name is required."
                });
            }

            if (model.StartDateTimeUtc >= model.EndDateTimeUtc)
            {
                return BadRequest(new
                {
                    message = "End date/time must be greater than start date/time."
                });
            }

            // Existing active competition ko deactivate karo
            var activeSettings = await _context.CompetitionSettings
                .Where(x => x.IsActive)
                .ToListAsync();

            foreach (var item in activeSettings)
            {
                item.IsActive = false;
                item.UpdatedAtUtc = DateTime.UtcNow;
            }

            var setting = new CompetitionSetting
            {
                Id = Guid.NewGuid(),

                CompetitionName = model.CompetitionName,

                StartDateTimeUtc = DateTime.SpecifyKind(
                    model.StartDateTimeUtc,
                    DateTimeKind.Utc),

                EndDateTimeUtc = DateTime.SpecifyKind(
                    model.EndDateTimeUtc,
                    DateTimeKind.Utc),

                IsActive = true,

                CreatedAtUtc = DateTime.UtcNow
            };

            _context.CompetitionSettings.Add(setting);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Competition setting saved successfully.",

                id = setting.Id,
                competitionName = setting.CompetitionName,
                startDateTimeUtc = setting.StartDateTimeUtc,
                endDateTimeUtc = setting.EndDateTimeUtc
            });
        }
    }
}