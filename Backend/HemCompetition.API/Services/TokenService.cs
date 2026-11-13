using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using HemCompetition.API.Data;
using HemCompetition.API.Domain;
using HemCompetition.API.Infrastructure;
using HemCompetition.API.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace HemCompetition.API.Services;

/// <summary>
/// Issues short-lived access tokens and rotating refresh tokens.
/// The access token is held in browser memory; the refresh token travels only in an HttpOnly,
/// SameSite=Strict cookie scoped to the refresh endpoint, so page scripts can never read it.
/// </summary>
public sealed class TokenService(AppDbContext db, IOptions<JwtOptions> options)
{
    public const string StudentCookie = "hc_refresh";
    public const string AdminCookie = "hc_admin_refresh";
    public const string StudentCookiePath = "/api/auth";
    public const string AdminCookiePath = "/api/admin/auth";

    private readonly JwtOptions _jwt = options.Value;

    public string StudentAccessToken(AuthUser user, Guid sessionId) => Write(
    [
        new(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new("UserId", user.UserId.ToString()),
        new(ClaimTypes.Name, user.Username),
        new("SessionId", sessionId.ToString()),
        new(TokenTypes.Claim, TokenTypes.Student)
    ], _jwt.AccessTokenMinutes);

    public string AdminAccessToken(AdminUser admin) => Write(
    [
        new(ClaimTypes.NameIdentifier, admin.Id.ToString()),
        new("AdminId", admin.Id.ToString()),
        new(ClaimTypes.Name, admin.Username),
        new(TokenTypes.Claim, TokenTypes.Admin)
    ], _jwt.AccessTokenMinutes);

    public string AppealToken(AuthUser user) => Write(
    [
        new("UserId", user.UserId.ToString()),
        new(ClaimTypes.Name, user.Username),
        new(TokenTypes.Claim, TokenTypes.Appeal)
    ], _jwt.AppealTokenMinutes);

    private string Write(IEnumerable<Claim> claims, int minutes)
    {
        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key)), SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(_jwt.Issuer, _jwt.Audience, claims,
            expires: DateTime.UtcNow.AddMinutes(minutes), signingCredentials: credentials);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>Creates a refresh token row (caller saves) and returns the raw value for the cookie.</summary>
    public string IssueRefresh(long? studentId, Guid? adminId, Guid? sessionId, Guid? familyId = null)
    {
        var raw = Convert.ToBase64String(RandomNumberGenerator.GetBytes(48));
        var now = DateTime.UtcNow;
        db.RefreshTokens.Add(new RefreshToken
        {
            Id = Guid.NewGuid(),
            TokenHash = Hash(raw),
            StudentId = studentId,
            AdminId = adminId,
            SessionId = sessionId,
            FamilyId = familyId ?? Guid.NewGuid(),
            CreatedAtUtc = now,
            ExpiresAtUtc = now.AddHours(adminId != null ? _jwt.AdminRefreshHours : _jwt.StudentRefreshHours)
        });
        return raw;
    }

    /// <summary>
    /// Validates and consumes a refresh token. Presenting a token that was already rotated means it
    /// was copied, so the whole family is revoked and the caller must sign in again.
    /// </summary>
    public async Task<RefreshToken?> ConsumeAsync(string? raw, bool admin)
    {
        if (string.IsNullOrEmpty(raw)) return null;
        var hash = Hash(raw);
        var now = DateTime.UtcNow;
        var token = await db.RefreshTokens.FirstOrDefaultAsync(t => t.TokenHash == hash);
        if (token == null || (token.AdminId != null) != admin) return null;
        if (token.RevokedAtUtc != null || token.ExpiresAtUtc <= now) return null;

        // Atomic claim: only one concurrent request can rotate a given token.
        var claimed = await db.RefreshTokens.Where(t => t.Id == token.Id && t.UsedAtUtc == null)
            .ExecuteUpdateAsync(u => u.SetProperty(t => t.UsedAtUtc, now));
        if (claimed == 0)
        {
            await RevokeFamilyAsync(token.FamilyId);
            return null;
        }
        return token;
    }

    public Task RevokeFamilyAsync(Guid familyId) =>
        db.RefreshTokens.Where(t => t.FamilyId == familyId && t.RevokedAtUtc == null)
            .ExecuteUpdateAsync(u => u.SetProperty(t => t.RevokedAtUtc, DateTime.UtcNow));

    public async Task RevokeByRawAsync(string? raw)
    {
        if (string.IsNullOrEmpty(raw)) return;
        var hash = Hash(raw);
        var family = await db.RefreshTokens.Where(t => t.TokenHash == hash).Select(t => (Guid?)t.FamilyId).FirstOrDefaultAsync();
        if (family != null) await RevokeFamilyAsync(family.Value);
    }

    public Task RevokeAllForAdminAsync(Guid adminId) =>
        db.RefreshTokens.Where(t => t.AdminId == adminId && t.RevokedAtUtc == null)
            .ExecuteUpdateAsync(u => u.SetProperty(t => t.RevokedAtUtc, DateTime.UtcNow));

    public void SetCookie(HttpResponse response, bool admin, string raw)
    {
        response.Cookies.Append(admin ? AdminCookie : StudentCookie, raw, new CookieOptions
        {
            HttpOnly = true,
            Secure = response.HttpContext.Request.IsHttps,
            SameSite = SameSiteMode.Strict,
            Path = admin ? AdminCookiePath : StudentCookiePath,
            Expires = DateTimeOffset.UtcNow.AddHours(admin ? _jwt.AdminRefreshHours : _jwt.StudentRefreshHours),
            IsEssential = true
        });
    }

    public static void ClearCookie(HttpResponse response, bool admin) =>
        response.Cookies.Delete(admin ? AdminCookie : StudentCookie,
            new CookieOptions { Path = admin ? AdminCookiePath : StudentCookiePath, SameSite = SameSiteMode.Strict, HttpOnly = true });

    public static string Hash(string raw) => Convert.ToHexStringLower(SHA256.HashData(Encoding.UTF8.GetBytes(raw)));
}
