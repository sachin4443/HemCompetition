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

namespace HemCompetition.API.Controllers.Admin;

public sealed class AdminLoginRequest
{
    [Required, StringLength(256)] public string Username { get; set; } = string.Empty;
    [Required, StringLength(200)] public string Password { get; set; } = string.Empty;
}

public sealed class ChangePasswordRequest
{
    [Required, StringLength(200)] public string CurrentPassword { get; set; } = string.Empty;
    [Required, StringLength(200, MinimumLength = 10)] public string NewPassword { get; set; } = string.Empty;
}

[ApiController]
[Route("api/admin/auth")]
public class AdminAuthController(
    AppDbContext db, TokenService tokens, AdminAccessService access, SettingsService settingsService, AuditService audit) : ControllerBase
{
    private static readonly PasswordHasher<AdminUser> Hasher = new();
    private static readonly string DummyHash = Hasher.HashPassword(new AdminUser(), Guid.NewGuid().ToString());

    [HttpPost("login")]
    [EnableRateLimiting("login")]
    public async Task<IActionResult> Login([FromBody] AdminLoginRequest model)
    {
        var settings = await settingsService.GetAsync();
        var username = model.Username.Trim();
        var admin = await db.AdminUsers.FirstOrDefaultAsync(x => x.Username == username);
        var now = DateTime.UtcNow;
        IActionResult Invalid() => Unauthorized(new { code = "INVALID_CREDENTIALS", message = "Invalid username or password." });

        if (admin == null)
        {
            Hasher.VerifyHashedPassword(new AdminUser(), DummyHash, model.Password);
            return Invalid();
        }
        if (admin.LockoutEndUtc != null && admin.LockoutEndUtc > now)
            return StatusCode(423, new { code = "LOCKED_OUT", message = "Too many failed attempts. Please try again later." });

        if (Hasher.VerifyHashedPassword(admin, admin.PasswordHash, model.Password) == PasswordVerificationResult.Failed)
        {
            admin.AccessFailedCount++;
            if (admin.AccessFailedCount >= settings.MaxFailedLogins)
            {
                admin.LockoutEndUtc = now.AddMinutes(settings.LockoutMinutes);
                admin.AccessFailedCount = 0;
            }
            admin.UpdatedAtUtc = now;
            audit.Add("admin.login_failed", "admin", admin.Id);
            await db.SaveChangesAsync();
            return Invalid();
        }
        if (!admin.IsActive) return Invalid();

        admin.AccessFailedCount = 0;
        admin.LockoutEndUtc = null;
        admin.LastLoginAtUtc = now;
        admin.UpdatedAtUtc = now;
        var refresh = tokens.IssueRefresh(null, admin.Id, null);
        db.AuditLogs.Add(new AuditLog
        {
            ActorType = "admin", ActorId = admin.Id.ToString(), ActorName = admin.Username, Action = "admin.login",
            IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(), CreatedAtUtc = now
        });
        await db.SaveChangesAsync();
        access.Invalidate(admin.Id);

        tokens.SetCookie(Response, admin: true, refresh);
        return Ok(await SessionPayload(admin));
    }

    [HttpPost("refresh")]
    [EnableRateLimiting("refresh")]
    public async Task<IActionResult> Refresh()
    {
        if (!Request.Headers.ContainsKey("X-Requested-With")) return BadRequest(new { message = "Missing header." });
        var token = await tokens.ConsumeAsync(Request.Cookies[TokenService.AdminCookie], admin: true);
        var admin = token?.AdminId == null ? null : await db.AdminUsers.AsNoTracking().FirstOrDefaultAsync(a => a.Id == token.AdminId);
        if (admin == null || !admin.IsActive)
        {
            TokenService.ClearCookie(Response, admin: true);
            return Unauthorized(new { code = "SESSION_EXPIRED", message = "Please sign in again." });
        }
        var refresh = tokens.IssueRefresh(null, admin.Id, null, token!.FamilyId);
        await db.SaveChangesAsync();
        tokens.SetCookie(Response, admin: true, refresh);
        return Ok(await SessionPayload(admin));
    }

    [Authorize(Policy = Policies.Admin)]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await tokens.RevokeByRawAsync(Request.Cookies[TokenService.AdminCookie]);
        TokenService.ClearCookie(Response, admin: true);
        return Ok(new { message = "Signed out." });
    }

    [Authorize(Policy = Policies.Admin)]
    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var admin = await db.AdminUsers.AsNoTracking().FirstOrDefaultAsync(a => a.Id == User.AdminId());
        if (admin == null || !admin.IsActive) return Unauthorized();
        var payload = await SessionPayload(admin, includeToken: false);
        return Ok(payload);
    }

    [Authorize(Policy = Policies.Admin)]
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest model)
    {
        var admin = await db.AdminUsers.FirstOrDefaultAsync(a => a.Id == User.AdminId());
        if (admin == null || !admin.IsActive) return Unauthorized();
        if (Hasher.VerifyHashedPassword(admin, admin.PasswordHash, model.CurrentPassword) == PasswordVerificationResult.Failed)
            return BadRequest(new { message = "The current password is incorrect." });
        var problem = PasswordRules.Check(model.NewPassword);
        if (problem != null) return BadRequest(new { message = problem });
        if (model.NewPassword == model.CurrentPassword)
            return BadRequest(new { message = "Choose a password you have not used here before." });

        admin.PasswordHash = Hasher.HashPassword(admin, model.NewPassword);
        admin.MustChangePassword = false;
        admin.UpdatedAtUtc = DateTime.UtcNow;
        audit.Add("admin.password_changed", "admin", admin.Id);
        await db.SaveChangesAsync();
        access.Invalidate(admin.Id);
        return Ok(new { message = "Password changed." });
    }

    private async Task<object> SessionPayload(AdminUser admin, bool includeToken = true)
    {
        access.Invalidate(admin.Id);
        var info = await access.GetAsync(admin.Id);
        return new
        {
            token = includeToken ? tokens.AdminAccessToken(admin) : null,
            profile = new
            {
                id = admin.Id,
                username = admin.Username,
                displayName = admin.DisplayName,
                role = info?.RoleName,
                mustChangePassword = admin.MustChangePassword,
                permissions = info?.Effective.OrderBy(p => p).ToArray() ?? []
            }
        };
    }
}

public static class PasswordRules
{
    /// <summary>Returns a message when the password is too weak, otherwise null.</summary>
    public static string? Check(string password)
    {
        if (password.Length < 10) return "Use at least 10 characters.";
        if (!password.Any(char.IsLetter) || !password.Any(char.IsDigit)) return "Use both letters and numbers.";
        return null;
    }
}
