namespace HemCompetition.API.Infrastructure;

public sealed class JwtOptions
{
    public string Key { get; set; } = string.Empty;
    public string Issuer { get; set; } = "HemCompetitionAPI";
    public string Audience { get; set; } = "HemCompetitionUsers";
    public int AccessTokenMinutes { get; set; } = 15;
    public int StudentRefreshHours { get; set; } = 8;
    public int AdminRefreshHours { get; set; } = 12;
    public int AppealTokenMinutes { get; set; } = 30;
}

public sealed class HeartbeatOptions
{
    public int HeartbeatIntervalSeconds { get; set; } = 30;
    public int HeartbeatTimeoutSeconds { get; set; } = 150;
}

public sealed class StorageOptions
{
    /// <summary>Directory for evidence and question images. Must be outside wwwroot.</summary>
    public string Root { get; set; } = "App_Data/storage";
}

public sealed class BootstrapOptions
{
    /// <summary>Creates the first Super Admin when the admin table is empty. Ignored afterwards.</summary>
    public string? AdminUsername { get; set; }
    public string? AdminPassword { get; set; }
    /// <summary>Loads the built-in starter word list when the word bank is empty.</summary>
    public bool SeedWordBank { get; set; } = true;
}

/// <summary>An expected failure that should reach the client as a normal error response.</summary>
public class ApiException(int statusCode, string message, string? code = null, object? extra = null) : Exception(message)
{
    public int StatusCode { get; } = statusCode;
    public string? Code { get; } = code;
    public object? Extra { get; } = extra;

    public static ApiException BadRequest(string message, string? code = null) => new(400, message, code);
    public static ApiException NotFound(string message = "Not found.") => new(404, message, "NOT_FOUND");
    public static ApiException Conflict(string message, string? code = null) => new(409, message, code);
    public static ApiException Forbidden(string message, string? code = null) => new(403, message, code);
}

public sealed record PagedResult<T>(IReadOnlyList<T> Items, int Total, int Page, int PageSize);

public sealed class PageQuery
{
    private int _page = 1, _pageSize = 25;
    public int Page { get => _page; set => _page = Math.Max(1, value); }
    public int PageSize { get => _pageSize; set => _pageSize = Math.Clamp(value, 1, 200); }
    public string? Search { get; set; }
    public int Skip => (Page - 1) * PageSize;
}
