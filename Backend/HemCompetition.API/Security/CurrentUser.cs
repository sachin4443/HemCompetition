using System.Security.Claims;

namespace HemCompetition.API.Security;

public static class TokenTypes
{
    public const string Claim = "typ";
    public const string Student = "student";
    public const string Admin = "admin";
    /// <summary>Issued to a banned student after a correct password. Only allows filing and tracking an appeal.</summary>
    public const string Appeal = "appeal";
}

public static class Policies
{
    public const string Student = "Student";
    public const string Admin = "Admin";
    public const string StudentOrAppeal = "StudentOrAppeal";
}

public static class CurrentUser
{
    public static string? TokenType(this ClaimsPrincipal user) => user.FindFirstValue(TokenTypes.Claim);
    public static bool IsAdmin(this ClaimsPrincipal user) => user.TokenType() == TokenTypes.Admin;
    public static bool IsStudent(this ClaimsPrincipal user) => user.TokenType() == TokenTypes.Student;

    /// <summary>The student's id (auth_users.user_id). Always taken from the token, never from the request.</summary>
    public static long StudentId(this ClaimsPrincipal user) =>
        long.TryParse(user.FindFirstValue("UserId"), out var id) ? id : throw new UnauthorizedAccessException();

    public static Guid SessionId(this ClaimsPrincipal user) =>
        Guid.TryParse(user.FindFirstValue("SessionId"), out var id) ? id : Guid.Empty;

    public static Guid AdminId(this ClaimsPrincipal user) =>
        Guid.TryParse(user.FindFirstValue("AdminId"), out var id) ? id : throw new UnauthorizedAccessException();

    public static string DisplayName(this ClaimsPrincipal user) => user.FindFirstValue(ClaimTypes.Name) ?? "unknown";
}
