using System.Security.Claims;
using HemCompetition.API.Data;
using HemCompetition.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HemCompetition.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CompetitionSecurityController : ControllerBase
    {
        private readonly AuthDbContext _context;

        private const string CurrentConsentVersion = "1.0";

        public CompetitionSecurityController(AuthDbContext context)
        {
            _context = context;
        }

        [HttpGet("status")]
        public async Task<IActionResult> GetStatus()
        {
            var userIdClaim = User.FindFirstValue("UserId");

            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized(new
                {
                    message = "Invalid token."
                });
            }

            if (!long.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new
                {
                    message = "Invalid UserId."
                });
            }

            var security = await _context.CompetitionSecurities
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (security == null)
            {
                return Ok(new
                {
                    exists = false,
                    termsAccepted = false,
                    consentVersion = CurrentConsentVersion,
                    status = "ACTIVE",
                    violationType = (string?)null,
                    violationMessage = (string?)null,
                    disqualifiedAtUtc = (DateTime?)null
                });
            }

            return Ok(new
            {
                exists = true,

                termsAccepted =
                    security.TermsAccepted &&
                    security.ConsentVersion == CurrentConsentVersion,

                consentVersion = security.ConsentVersion,

                status = security.Status,

                violationType = security.ViolationType,

                violationMessage = security.ViolationMessage,

                disqualifiedAtUtc = security.DisqualifiedAtUtc,

                lastCameraStatus = security.LastCameraStatus,

                lastPersonCount = security.LastPersonCount
            });
        }

        [HttpPost("accept")]
        public async Task<IActionResult> AcceptTerms()
        {
            var userIdClaim = User.FindFirstValue("UserId");

            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized(new
                {
                    message = "Invalid token."
                });
            }

            if (!long.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new
                {
                    message = "Invalid UserId."
                });
            }

            var security = await _context.CompetitionSecurities
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (security != null &&
                security.Status == "DISQUALIFIED")
            {
                return BadRequest(new
                {
                    message = "You are disqualified from this competition.",
                    status = "DISQUALIFIED"
                });
            }

            if (security == null)
            {
                security = new CompetitionSecurity
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,

                    ConsentVersion = CurrentConsentVersion,

                    TermsAccepted = true,

                    TermsAcceptedAtUtc = DateTime.UtcNow,

                    Status = "ACTIVE",

                    ViolationCount = 0,

                    CreatedAtUtc = DateTime.UtcNow,

                    UpdatedAtUtc = DateTime.UtcNow
                };

                _context.CompetitionSecurities.Add(security);
            }
            else
            {
                security.ConsentVersion = CurrentConsentVersion;

                security.TermsAccepted = true;

                security.TermsAcceptedAtUtc = DateTime.UtcNow;

                security.UpdatedAtUtc = DateTime.UtcNow;

                if (security.Status != "DISQUALIFIED")
                {
                    security.Status = "ACTIVE";
                }
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,

                message = "Competition rules accepted successfully.",

                termsAccepted = true,

                status = security.Status
            });
        }

        [HttpPost("camera-status")]
        public async Task<IActionResult> UpdateCameraStatus([FromBody] CameraStatusRequest request)
        {
            var userIdClaim = User.FindFirstValue("UserId");

            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized();

            if (!long.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var security = await _context.CompetitionSecurities
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (security == null)
            {
                return BadRequest(new
                {
                    message = "Competition security record not found."
                });
            }

            if (security.Status == "DISQUALIFIED")
            {
                return Ok(new
                {
                    status = "DISQUALIFIED",
                    message = "You are disqualified from this competition."
                });
            }

            security.LastCameraStatus = request.CameraStatus;
            security.LastPersonCount = request.PersonCount;
            security.UpdatedAtUtc = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                status = security.Status
            });
        }

        [HttpPost("disqualify")]
        public async Task<IActionResult> Disqualify([FromBody] DisqualifyRequest request)
        {
            var userIdClaim = User.FindFirstValue("UserId");

            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized();

            if (!long.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var security = await _context.CompetitionSecurities
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (security == null)
            {
                return BadRequest(new
                {
                    message = "Competition security record not found."
                });
            }

            if (security.Status == "DISQUALIFIED")
            {
                return Ok(new
                {
                    success = true,
                    status = "DISQUALIFIED"
                });
            }

            security.Status = "DISQUALIFIED";

            security.ViolationType = request.ViolationType;

            security.ViolationMessage = request.ViolationMessage;

            security.ViolationCount++;

            security.DisqualifiedAtUtc = DateTime.UtcNow;

            security.LastViolationAtUtc = DateTime.UtcNow;

            security.LastCameraStatus = request.CameraStatus;

            security.LastPersonCount = request.PersonCount;

            security.UpdatedAtUtc = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                status = "DISQUALIFIED",
                message = "You have been disqualified from this competition."
            });
        }


    }
}