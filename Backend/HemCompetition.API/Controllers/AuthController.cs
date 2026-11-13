using System.ComponentModel.DataAnnotations;
using HemCompetition.API.Data;
using HemCompetition.API.Domain;
using HemCompetition.API.Security;
using HemCompetition.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;

namespace HemCompetition.API.Controllers;

public sealed class LoginRequest
{
    [Required, StringLength(256)] public string UsernameOrEmail { get; set; } = string.Empty;
    [Required, StringLength(200)] public string Password { get; set; } = string.Empty;
    /// <summary>Set after the student confirms they want to end a session that is open elsewhere.</summary>
    public bool TerminateExisting { get; set; }
    [StringLength(64)] public string? CaptchaId { get; set; }
    [StringLength(16)] public string? CaptchaAnswer { get; set; }
}

[ApiController]
[Route("api/auth")]
public class AuthController(
    AppDbContext db, TokenService tokens, SessionService sessions, BanService bans,
    SettingsService settingsService, ProctoringService proctoring, CaptchaService captcha) : ControllerBase
{
    private static readonly PasswordHasher<AuthUser> Hasher = new();
    // Verified when the username is unknown so both paths take the same time.
    private static readonly string DummyHash = Hasher.HashPassword(new AuthUser(), Guid.NewGuid().ToString());

    /// <summary>A new captcha for the sign-in form. Null image when the captcha is switched off.</summary>
    [HttpGet("captcha")]
    [EnableRateLimiting("login")]
    public async Task<IActionResult> Captcha()
    {
        if (!(await settingsService.GetAsync()).LoginCaptchaEnabled) return Ok(new { enabled = false });
        var challenge = captcha.Create();
        return Ok(new
        {
            enabled = true,
            id = challenge.Id,
            image = "data:image/svg+xml;base64," + Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(challenge.Svg))
        });
    }

    [HttpPost("login")]
    [EnableRateLimiting("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest model)
    {
        var settings = await settingsService.GetAsync();
        if (settings.LoginCaptchaEnabled)
        {
            // Checked before the password so a script cannot test passwords without solving it.
            if (!captcha.Check(model.CaptchaId, model.CaptchaAnswer))
                return BadRequest(new { code = "CAPTCHA_INVALID", message = "The verification code is not correct. Please try the new code." });
        }
        // The "already signed in elsewhere" answer asks the student to confirm and resend, so the
        // same captcha stays valid for that one case; every other outcome uses it up.
        var keepCaptcha = false;
        try
        {
            return await LoginCore(model, settings, () => keepCaptcha = true);
        }
        finally
        {
            if (!keepCaptcha) captcha.Consume(model.CaptchaId);
        }
    }

    private async Task<IActionResult> LoginCore(LoginRequest model, PlatformSettings settings, Action keepCaptcha)
    {
        var login = model.UsernameOrEmail.Trim();
        var user = await db.AuthUsers.FirstOrDefaultAsync(u => u.Username == login || u.Email == login);
        var now = DateTime.UtcNow;

        // One message for "unknown user" and "wrong password": usernames are phone numbers and must not be discoverable.
        IActionResult Invalid() => Unauthorized(new
        {
            code = "INVALID_CREDENTIALS",
            message = "The username or password is incorrect, or this account is not registered for the competition."
        });

        if (user == null)
        {
            Hasher.VerifyHashedPassword(new AuthUser(), DummyHash, model.Password);
            return Invalid();
        }

        if (user.LockoutEndDateUtc != null && user.LockoutEndDateUtc > now)
            return StatusCode(423, new
            {
                code = "LOCKED_OUT",
                message = "Too many failed attempts. Please try again later.",
                retryAfterSeconds = (int)(user.LockoutEndDateUtc.Value - now).TotalSeconds
            });

        var verification = Hasher.VerifyHashedPassword(user, user.PasswordHash, model.Password);
        if (verification == PasswordVerificationResult.Failed)
        {
            user.AccessFailedCount++;
            if (user.AccessFailedCount >= settings.MaxFailedLogins)
            {
                user.LockoutEndDateUtc = now.AddMinutes(settings.LockoutMinutes);
                user.AccessFailedCount = 0;
            }
            user.UpdatedAtUtc = now;
            await db.SaveChangesAsync();
            return Invalid();
        }

        if (!user.IsActive || user.IsUserDeactivated)
            return StatusCode(403, new { code = "ACCOUNT_INACTIVE", message = "Your account is inactive. Please contact the competition administrator." });

        var ban = await bans.GetActiveAsync(user.UserId);
        if (ban != null)
        {
            var appeal = await db.Appeals.AsNoTracking().Where(a => a.BanId == ban.Id)
                .OrderByDescending(a => a.SubmittedAtUtc)
                .Select(a => new { a.Id, status = a.Status.ToString(), a.SubmittedAtUtc, a.DecidedAtUtc, a.DecisionReason })
                .FirstOrDefaultAsync();
            return StatusCode(403, new
            {
                code = "BANNED",
                message = ban.Type == BanType.Ban
                    ? "Your access to this competition has been restricted."
                    : "Your access to this competition is temporarily suspended.",
                ban = BanService.Describe(ban),
                appeal,
                appealsEnabled = settings.AppealsEnabled,
                // Lets the student file or track an appeal without having a normal session.
                appealToken = tokens.AppealToken(user)
            });
        }

        var live = await sessions.GetLiveAsync(user.UserId);
        if (live != null && !model.TerminateExisting)
        {
            keepCaptcha();
            return Conflict(new
            {
                code = "SESSION_ACTIVE",
                requiresSessionTermination = true,
                message = "You are already signed in on another device or tab."
            });
        }

        var session = await sessions.CreateAsync(user.UserId,
            HttpContext.Connection.RemoteIpAddress?.ToString(), Request.Headers.UserAgent.ToString());
        var refresh = tokens.IssueRefresh(user.UserId, null, session.SessionId);
        if (verification == PasswordVerificationResult.SuccessRehashNeeded)
            user.PasswordHash = Hasher.HashPassword(user, model.Password);
        user.LastLoginAtUtc = now;
        user.AccessFailedCount = 0;
        user.LockoutEndDateUtc = null;
        user.UpdatedAtUtc = now;
        await db.SaveChangesAsync();

        if (live != null)
            await proctoring.RecordSystemEventAsync(user.UserId, "MULTIPLE_SESSION", "Signed in while another session was still active.");

        tokens.SetCookie(Response, admin: false, refresh);
        return Ok(new { token = tokens.StudentAccessToken(user, session.SessionId), profile = Profile(user) });
    }

    /// <summary>Exchanges the refresh cookie for a new access token and rotates the cookie.</summary>
    [HttpPost("refresh")]
    [EnableRateLimiting("refresh")]
    public async Task<IActionResult> Refresh()
    {
        // Custom header that a cross-site form post cannot send.
        if (!Request.Headers.ContainsKey("X-Requested-With")) return BadRequest(new { message = "Missing header." });

        var token = await tokens.ConsumeAsync(Request.Cookies[TokenService.StudentCookie], admin: false);
        if (token?.StudentId == null || token.SessionId == null ||
            !await sessions.IsValidAsync(token.StudentId.Value, token.SessionId.Value))
        {
            TokenService.ClearCookie(Response, admin: false);
            return Unauthorized(new { code = "SESSION_EXPIRED", message = "Please sign in again." });
        }
        var user = await db.AuthUsers.AsNoTracking().FirstAsync(u => u.UserId == token.StudentId);
        if (!user.IsActive || user.IsUserDeactivated || await bans.GetActiveAsync(user.UserId) != null)
        {
            await sessions.TerminateAsync(user.UserId);
            TokenService.ClearCookie(Response, admin: false);
            return Unauthorized(new { code = "SESSION_EXPIRED", message = "Please sign in again." });
        }
        var refresh = tokens.IssueRefresh(user.UserId, null, token.SessionId, token.FamilyId);
        await db.SaveChangesAsync();
        tokens.SetCookie(Response, admin: false, refresh);
        return Ok(new { token = tokens.StudentAccessToken(user, token.SessionId.Value), profile = Profile(user) });
    }

    [Authorize(Policy = Policies.Student)]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await sessions.TerminateAsync(User.StudentId());
        TokenService.ClearCookie(Response, admin: false);
        return Ok(new { message = "Signed out." });
    }

    [Authorize(Policy = Policies.Student)]
    [HttpPost("heartbeat")]
    public async Task<IActionResult> Heartbeat()
    {
        if (!await sessions.HeartbeatAsync(User.StudentId(), User.SessionId()))
            return Unauthorized(new { code = "SESSION_EXPIRED", message = "Session expired." });
        return Ok(new { active = true, serverTimeUtc = DateTime.UtcNow });
    }

    /// <summary>Whether the signed-in (or appealing) student is currently restricted.</summary>
    [Authorize(Policy = Policies.StudentOrAppeal)]
    [HttpGet("account-status")]
    public async Task<IActionResult> AccountStatus()
    {
        var ban = await bans.GetActiveAsync(User.StudentId());
        return Ok(new { restricted = ban != null, ban = ban == null ? null : BanService.Describe(ban) });
    }

    private static object Profile(AuthUser u) => new
    {
        userId = u.UserId,
        username = u.Username,
        firstName = u.FirstName,
        lastName = u.LastName,
        email = u.EmailId ?? u.Email,
        mobileNo = u.PhoneNumber,
        city = u.City
    };
}
