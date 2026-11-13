using HemCompetition.API.DTOs;
using HemCompetition.API.Models;
using HemCompetition.API.Services; // Service namespace
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HemCompetition.API.Data;

namespace HemCompetition.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly Hemvirtues2627Context _context;
        private readonly IConfiguration _configuration;
        private readonly IAuthSessionService _authSessionService;
        private readonly AuthDbContext _authContext;

        public AuthController(
    Hemvirtues2627Context context,
    AuthDbContext authContext,
    IConfiguration configuration,
    IAuthSessionService authSessionService)
        {
            _context = context;
            _authContext = authContext;
            _configuration = configuration;
            _authSessionService = authSessionService;
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            if (model == null ||
                string.IsNullOrWhiteSpace(model.UsernameOrEmail) ||
                string.IsNullOrWhiteSpace(model.Password))
            {
                return BadRequest(new
                {
                    message = "Username/Email and Password are required."
                });
            }


            var loginValue = model.UsernameOrEmail.Trim();
            var authUser = await _authContext.AuthUsers.FirstOrDefaultAsync(u =>u.Username == loginValue ||u.Email == loginValue);

            if (authUser == null)
            {
                return NotFound(new
                {
                    loginErrorType = "NOT_ELIGIBLE",
                    message = "You are not eligible for this competition."
                });
            }

            if (!authUser.IsActive ||
                authUser.IsUserDeactivated)
            {
                return Unauthorized(new
                {
                    message = "Your account is inactive."
                });
            }
            var passwordHasher =
                new PasswordHasher<AuthUser>();

            var verificationResult =
                passwordHasher.VerifyHashedPassword(
                    authUser,
                    authUser.PasswordHash,
                    model.Password);

            if (verificationResult ==
    PasswordVerificationResult.Failed)
            {
                authUser.AccessFailedCount++;

                authUser.UpdatedAtUtc =
                    DateTime.UtcNow;

                await _authContext.SaveChangesAsync();

                return Unauthorized(new
                {
                    loginErrorType = "INVALID_CREDENTIALS",
                    message = "Please check your credentials."
                });
            }
            var competitionSecurity =
                await _authContext.CompetitionSecurities
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x =>
                        x.UserId == authUser.UserId);


            if (competitionSecurity != null &&
                string.Equals(
                    competitionSecurity.Status,
                    "DISQUALIFIED",
                    StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(new
                {
                    disqualified = true,

                    status = "DISQUALIFIED",

                    message =
                        "You are disqualified from this competition and cannot login again.",

                    violationType =
                        competitionSecurity.ViolationType,

                    violationMessage =
                        competitionSecurity.ViolationMessage,

                    disqualifiedAtUtc =
                        competitionSecurity.DisqualifiedAtUtc
                });
            }
            var activeSession =
                await _authSessionService
                    .GetActiveSessionAsync(
                        authUser.UserId);


            if (activeSession != null)
            {
                return BadRequest(new
                {
                    requiresSessionTermination = true,

                    userId = authUser.UserId,

                    message =
                        "User is already logged in on another device or tab."
                });
            }

            var sessionId =
                await _authSessionService
                    .CreateNewSessionAsync(
                        authUser.UserId);
            authUser.LastLoginAtUtc =
                DateTime.UtcNow;

            authUser.AccessFailedCount = 0;

            authUser.UpdatedAtUtc =
                DateTime.UtcNow;

            await _authContext.SaveChangesAsync();

            var tokenHandler =
                new JwtSecurityTokenHandler();


            var jwtKey =
    _configuration["Jwt:Key"];

            if (string.IsNullOrWhiteSpace(jwtKey))
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new
                    {
                        message =
                            "Authentication configuration is invalid."
                    });
            }


            var key =
                Encoding.ASCII.GetBytes(jwtKey);

            var claims = new List<Claim>
    {
        new Claim(
            ClaimTypes.NameIdentifier,
            authUser.Id.ToString()),

        new Claim(
            "UserId",
            authUser.UserId.ToString()),

        new Claim(
            ClaimTypes.Name,
            authUser.Username),

        new Claim(
            ClaimTypes.Email,
            authUser.Email ?? string.Empty),

        new Claim(
            "SessionId",
            sessionId.ToString())
    };

            if (!double.TryParse(
                    _configuration["Jwt:ExpireMinutes"],
                    out var expireMinutes))
            {
                expireMinutes = 60;
            }


            var tokenDescriptor =
                new SecurityTokenDescriptor
                {
                    Subject =
                        new ClaimsIdentity(claims),

                    Expires =
                        DateTime.UtcNow.AddMinutes(
                            expireMinutes),

                    Issuer =
                        _configuration["Jwt:Issuer"],

                    Audience =
                        _configuration["Jwt:Audience"],

                    SigningCredentials =
                        new SigningCredentials(
                            new SymmetricSecurityKey(key),
                            SecurityAlgorithms.HmacSha256Signature)
                };

            var token =
                tokenHandler.CreateToken(
                    tokenDescriptor);


            var tokenString =
                tokenHandler.WriteToken(token);
            return Ok(new
            {
                token = tokenString,

                message = "Login Successful",

                profile = new
                {
                    userId = authUser.UserId,

                    firstName = authUser.FirstName,

                    lastName = authUser.LastName,

                    email =
                        authUser.EmailId ??
                        authUser.Email,

                    mobileNo =
                        authUser.PhoneNumber,

                    address =
                        authUser.AddressLine1,

                    city =
                        authUser.City,

                    pincode =
                        authUser.Pincode
                }
            });
        }

        [HttpPost("terminate-session")]
        public async Task<IActionResult> TerminateAndLogin( [FromBody] TerminateSessionDto model)
        {
            if (model.UserId <= 0)
            {
                return BadRequest(new
                {
                    message = "Invalid UserId."
                });
            }


            await _authSessionService
                .TerminateSessionAsync(model.UserId);


            return Ok(new
            {
                message =
                    "Previous session terminated successfully. You can now login."
            });
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var userIdClaim =
                User.FindFirst("UserId")?.Value;


            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized(new
                {
                    message = "Invalid user session."
                });
            }


            if (!int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new
                {
                    message = "Invalid UserId."
                });
            }


            await _authSessionService
                .TerminateSessionAsync(userId);


            return Ok(new
            {
                message = "Logged out successfully"
            });
        }
        [Authorize]
        [HttpPost("heartbeat")]
        public async Task<IActionResult> Heartbeat()
        {
            var userIdClaim =
                User.FindFirst("UserId")?.Value;

            var sessionIdClaim =
                User.FindFirst("SessionId")?.Value;


            if (string.IsNullOrEmpty(userIdClaim) ||
                string.IsNullOrEmpty(sessionIdClaim))
            {
                return Unauthorized(new
                {
                    message = "Invalid session."
                });
            }


            if (!long.TryParse(
                    userIdClaim,
                    out var userId))
            {
                return Unauthorized(new
                {
                    message = "Invalid UserId."
                });
            }


            if (!Guid.TryParse(
                    sessionIdClaim,
                    out var sessionId))
            {
                return Unauthorized(new
                {
                    message = "Invalid SessionId."
                });
            }


            var success =
                await _authSessionService
                    .UpdateHeartbeatAsync(
                        userId,
                        sessionId);


            if (!success)
            {
                return Unauthorized(new
                {
                    message = "Session expired."
                });
            }


            return Ok(new
            {
                active = true,
                serverTime = DateTime.UtcNow
            });
        }

        [HttpGet("auth-db-test")]
        public async Task<IActionResult> AuthDbTest([FromServices] AuthDbContext authContext)
        {
            var user = await authContext.AuthUsers
                .FirstOrDefaultAsync(x => x.UserId == 655888);

            if (user == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "PostgreSQL user not found."
                });
            }

            return Ok(new
            {
                success = true,
                userId = user.UserId,
                username = user.Username,
                email = user.Email,
                firstName = user.FirstName,
                lastName = user.LastName
            });
        }
    }
}