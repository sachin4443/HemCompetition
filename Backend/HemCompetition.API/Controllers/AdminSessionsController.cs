using HemCompetition.API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace HemCompetition.API.Controllers
{
    [Route("api/admin-sessions")]
    [ApiController]
    [Authorize(Policy = "AdminOnly")]
    public class AdminSessionsController : ControllerBase
    {
        private readonly AuthDbContext _context;

        public AdminSessionsController(AuthDbContext context)
        {
            _context = context;
        }

        // =====================================================
        // LIST SESSIONS
        // =====================================================

        [HttpGet]
        public async Task<IActionResult> GetSessions(
            [FromQuery] string? search = null,
            [FromQuery] string? status = null,
            [FromQuery] DateTime? from = null,
            [FromQuery] DateTime? to = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50)
        {
            if (page < 1)
                page = 1;

            if (pageSize < 1)
                pageSize = 50;

            if (pageSize > 200)
                pageSize = 200;

            // =================================================
            // BASE QUERY
            // =================================================

            var query =
                from session in _context.AuthSessions.AsNoTracking()

                join user in _context.AuthUsers.AsNoTracking()
                    on session.UserId equals user.UserId

                select new
                {
                    session,
                    user
                };

            // =================================================
            // SEARCH
            // Username
            // Full Name
            // First Name
            // Last Name
            // Email
            // User ID
            // Session ID
            // IP
            // =================================================

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();

                var searchLower =
                    search.ToLower();

                query = query.Where(x =>

                    // Username
                    x.user.Username
                        .ToLower()
                        .Contains(searchLower)

                    ||

                    // Email
                    (
                        x.user.Email != null &&
                        x.user.Email
                            .ToLower()
                            .Contains(searchLower)
                    )

                    ||

                    // First Name
                    (
                        x.user.FirstName != null &&
                        x.user.FirstName
                            .ToLower()
                            .Contains(searchLower)
                    )

                    ||

                    // Last Name
                    (
                        x.user.LastName != null &&
                        x.user.LastName
                            .ToLower()
                            .Contains(searchLower)
                    )

                    ||

                    // Full Name
                    (
                        (
                            (x.user.FirstName ?? "") +
                            " " +
                            (x.user.LastName ?? "")
                        )
                        .ToLower()
                        .Contains(searchLower)
                    )

                    ||

                    // User ID
                    x.session.UserId
                        .ToString()
                        .Contains(search)

                    ||

                    // Session ID
                    x.session.SessionId
                        .ToString()
                        .Contains(search)

                    ||

                    // IP
                    (
                        x.session.IpAddress != null &&
                        x.session.IpAddress
                            .Contains(search)
                    )
                );
            }

            // =================================================
            // STATUS
            // =================================================

            if (!string.IsNullOrWhiteSpace(status))
            {
                if (status.Equals(
                    "active",
                    StringComparison.OrdinalIgnoreCase))
                {
                    query =
                        query.Where(x =>
                            x.session.IsActive);
                }

                if (status.Equals(
                    "loggedout",
                    StringComparison.OrdinalIgnoreCase))
                {
                    query =
                        query.Where(x =>
                            !x.session.IsActive);
                }
            }

            // =================================================
            // DATE FILTER
            // =================================================

            if (from.HasValue)
            {
                var fromUtc =
                    from.Value.Kind ==
                    DateTimeKind.Utc
                        ? from.Value
                        : from.Value.ToUniversalTime();

                query =
                    query.Where(x =>
                        x.session.CreatedAtUtc >=
                        fromUtc);
            }

            if (to.HasValue)
            {
                var toUtc =
                    to.Value.Kind ==
                    DateTimeKind.Utc
                        ? to.Value
                        : to.Value.ToUniversalTime();

                query =
                    query.Where(x =>
                        x.session.CreatedAtUtc <
                        toUtc.AddDays(1));
            }

            // =================================================
            // TOTAL
            // =================================================

            var total =
                await query.CountAsync();

            // =================================================
            // PAGINATION
            // =================================================

            var rows =
                await query
                    .OrderByDescending(x =>
                        x.session.CreatedAtUtc)
                    .Skip(
                        (page - 1) *
                        pageSize)
                    .Take(pageSize)
                    .ToListAsync();

            // =================================================
            // CURRENT TIME
            // =================================================

            var now =
                DateTime.UtcNow;

            // =================================================
            // RESPONSE DATA
            // =================================================

            var data =
                rows.Select(x =>
                {
                    var logout =
                        x.session.TerminatedAtUtc;

                    var end =
                        logout ?? now;

                    var duration =
                        end >
                        x.session.CreatedAtUtc
                            ? end -
                              x.session.CreatedAtUtc
                            : TimeSpan.Zero;

                    // =========================================
                    // FULL NAME
                    // =========================================

                    var firstName =
                        x.user.FirstName?.Trim();

                    var lastName =
                        x.user.LastName?.Trim();

                    var fullName =
                        $"{firstName} {lastName}"
                            .Trim();

                    // =========================================
                    // FALLBACK NAME
                    // =========================================

                    if (string.IsNullOrWhiteSpace(
                        fullName))
                    {
                        fullName =
                            x.user.Username;
                    }

                    return new
                    {
                        // =====================================
                        // SESSION IDENTIFICATION
                        // =====================================

                        sessionId =
                            x.session.SessionId,

                        sessionRecordId =
                            x.session.Id,

                        userId =
                            x.user.UserId,

                        // =====================================
                        // USER DETAILS
                        // =====================================

                        username =
                            x.user.Username,

                        firstName,

                        lastName,

                        fullName,

                        email =
                            x.user.Email,

                        // =====================================
                        // SESSION TIME
                        // =====================================

                        loginAtUtc =
                            x.session.CreatedAtUtc,

                        lastActivityUtc =
                            x.session.LastActivityUtc,

                        lastHeartbeatUtc =
                            x.session.LastHeartbeatUtc,

                        logoutAtUtc =
                            logout,

                        // =====================================
                        // DURATION
                        // =====================================

                        durationSeconds =
                            (long)
                            duration.TotalSeconds,

                        duration =
                            FormatDuration(
                                duration),

                        // =====================================
                        // STATUS
                        // =====================================

                        isActive =
                            x.session.IsActive,

                        status =
                            x.session.IsActive
                                ? "Active"
                                : "Logged Out",

                        // =====================================
                        // NETWORK / DEVICE
                        // =====================================

                        ipAddress =
                            x.session.IpAddress,

                        userAgent =
                            x.session.UserAgent
                    };
                })
                .ToList();

            return Ok(new
            {
                success = true,

                total,

                page,

                pageSize,

                totalPages =
                    (int)Math.Ceiling(
                        total /
                        (double)pageSize),

                data
            });
        }


        // =====================================================
        // SESSION STATS
        // =====================================================

        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            var today =
                DateTime.UtcNow.Date;

            var tomorrow =
                today.AddDays(1);

            var totalSessions =
                await _context.AuthSessions
                    .AsNoTracking()
                    .CountAsync();

            var activeSessions =
                await _context.AuthSessions
                    .AsNoTracking()
                    .CountAsync(x =>
                        x.IsActive);

            var loggedOutSessions =
                await _context.AuthSessions
                    .AsNoTracking()
                    .CountAsync(x =>
                        !x.IsActive);

            var todaySessions =
                await _context.AuthSessions
                    .AsNoTracking()
                    .CountAsync(x =>
                        x.CreatedAtUtc >= today &&
                        x.CreatedAtUtc < tomorrow);

            var recentSessions =
                await _context.AuthSessions
                    .AsNoTracking()
                    .Where(x =>
                        x.TerminatedAtUtc != null)
                    .Select(x => new
                    {
                        x.CreatedAtUtc,
                        x.TerminatedAtUtc
                    })
                    .Take(10000)
                    .ToListAsync();

            double averageDurationSeconds = 0;

            if (recentSessions.Count > 0)
            {
                averageDurationSeconds =
                    recentSessions
                        .Where(x =>
                            x.TerminatedAtUtc.HasValue &&
                            x.TerminatedAtUtc.Value >
                            x.CreatedAtUtc)
                        .Select(x =>
                            (
                                x.TerminatedAtUtc!.Value -
                                x.CreatedAtUtc
                            ).TotalSeconds)
                        .DefaultIfEmpty(0)
                        .Average();
            }

            return Ok(new
            {
                success = true,

                totalSessions,

                activeSessions,

                loggedOutSessions,

                todaySessions,

                averageDurationSeconds =
                    Math.Round(
                        averageDurationSeconds,
                        0)
            });
        }


        // =====================================================
        // SESSION DETAILS
        // =====================================================

        [HttpGet("{sessionId}")]
        public async Task<IActionResult> GetSession(
            Guid sessionId)
        {
            var result =
                await (
                    from session in
                        _context.AuthSessions
                            .AsNoTracking()

                    join user in
                        _context.AuthUsers
                            .AsNoTracking()

                        on session.UserId
                        equals user.UserId

                    where
                        session.SessionId ==
                        sessionId

                    select new
                    {
                        session,
                        user
                    }
                )
                .FirstOrDefaultAsync();

            if (result == null)
            {
                return NotFound(new
                {
                    success = false,
                    message =
                        "Session not found."
                });
            }

            var now =
                DateTime.UtcNow;

            var end =
                result.session.TerminatedAtUtc
                ?? now;

            var duration =
                end >
                result.session.CreatedAtUtc
                    ? end -
                      result.session.CreatedAtUtc
                    : TimeSpan.Zero;

            // =================================================
            // FULL NAME
            // =================================================

            var firstName =
                result.user.FirstName?.Trim();

            var lastName =
                result.user.LastName?.Trim();

            var fullName =
                $"{firstName} {lastName}"
                    .Trim();

            if (string.IsNullOrWhiteSpace(
                fullName))
            {
                fullName =
                    result.user.Username;
            }

            return Ok(new
            {
                success = true,

                data = new
                {
                    // =========================================
                    // SESSION
                    // =========================================

                    sessionId =
                        result.session.SessionId,

                    sessionRecordId =
                        result.session.Id,

                    userId =
                        result.user.UserId,

                    // =========================================
                    // USER
                    // =========================================

                    username =
                        result.user.Username,

                    firstName,

                    lastName,

                    fullName,

                    email =
                        result.user.Email,

                    phoneNumber =
                        result.user.PhoneNumber,

                    // =========================================
                    // SESSION TIME
                    // =========================================

                    loginAtUtc =
                        result.session.CreatedAtUtc,

                    lastActivityUtc =
                        result.session.LastActivityUtc,

                    lastHeartbeatUtc =
                        result.session.LastHeartbeatUtc,

                    logoutAtUtc =
                        result.session.TerminatedAtUtc,

                    // =========================================
                    // DURATION
                    // =========================================

                    durationSeconds =
                        (long)
                        duration.TotalSeconds,

                    duration =
                        FormatDuration(
                            duration),

                    // =========================================
                    // STATUS
                    // =========================================

                    isActive =
                        result.session.IsActive,

                    status =
                        result.session.IsActive
                            ? "Active"
                            : "Logged Out",

                    // =========================================
                    // NETWORK / DEVICE
                    // =========================================

                    ipAddress =
                        result.session.IpAddress,

                    userAgent =
                        result.session.UserAgent
                }
            });
        }


        // =====================================================
        // EXPORT CSV
        // =====================================================

        [HttpGet("export")]
        public async Task<IActionResult> Export(
            [FromQuery] string? search = null,
            [FromQuery] string? status = null,
            [FromQuery] DateTime? from = null,
            [FromQuery] DateTime? to = null)
        {
            var query =
                from session in
                    _context.AuthSessions
                        .AsNoTracking()

                join user in
                    _context.AuthUsers
                        .AsNoTracking()

                    on session.UserId
                    equals user.UserId

                select new
                {
                    session,
                    user
                };

            // =================================================
            // SEARCH
            // =================================================

            if (!string.IsNullOrWhiteSpace(
                search))
            {
                search =
                    search.Trim();

                var searchLower =
                    search.ToLower();

                query =
                    query.Where(x =>

                        // Username
                        x.user.Username
                            .ToLower()
                            .Contains(
                                searchLower)

                        ||

                        // Email
                        (
                            x.user.Email != null &&
                            x.user.Email
                                .ToLower()
                                .Contains(
                                    searchLower)
                        )

                        ||

                        // First Name
                        (
                            x.user.FirstName != null &&
                            x.user.FirstName
                                .ToLower()
                                .Contains(
                                    searchLower)
                        )

                        ||

                        // Last Name
                        (
                            x.user.LastName != null &&
                            x.user.LastName
                                .ToLower()
                                .Contains(
                                    searchLower)
                        )

                        ||

                        // Full Name
                        (
                            (
                                (x.user.FirstName ?? "") +
                                " " +
                                (x.user.LastName ?? "")
                            )
                            .ToLower()
                            .Contains(
                                searchLower)
                        )

                        ||

                        // User ID
                        x.session.UserId
                            .ToString()
                            .Contains(search)

                        ||

                        // Session ID
                        x.session.SessionId
                            .ToString()
                            .Contains(search)

                        ||

                        // IP
                        (
                            x.session.IpAddress != null &&
                            x.session.IpAddress
                                .Contains(search)
                        )
                    );
            }

            // =================================================
            // STATUS
            // =================================================

            if (status?.Equals(
                    "active",
                    StringComparison.OrdinalIgnoreCase)
                == true)
            {
                query =
                    query.Where(x =>
                        x.session.IsActive);
            }

            if (status?.Equals(
                    "loggedout",
                    StringComparison.OrdinalIgnoreCase)
                == true)
            {
                query =
                    query.Where(x =>
                        !x.session.IsActive);
            }

            // =================================================
            // DATE
            // =================================================

            if (from.HasValue)
            {
                var fromUtc =
                    from.Value.Kind ==
                    DateTimeKind.Utc
                        ? from.Value
                        : from.Value.ToUniversalTime();

                query =
                    query.Where(x =>
                        x.session.CreatedAtUtc >=
                        fromUtc);
            }

            if (to.HasValue)
            {
                var toUtc =
                    to.Value.Kind ==
                    DateTimeKind.Utc
                        ? to.Value
                        : to.Value.ToUniversalTime();

                query =
                    query.Where(x =>
                        x.session.CreatedAtUtc <
                        toUtc.AddDays(1));
            }

            // =================================================
            // FETCH
            // =================================================

            var rows =
                await query
                    .OrderByDescending(x =>
                        x.session.CreatedAtUtc)
                    .ToListAsync();

            var now =
                DateTime.UtcNow;

            // =================================================
            // CSV
            // =================================================

            var csv =
                new StringBuilder();

            csv.AppendLine(
                "UserId,FullName,FirstName,LastName,Username,Email,PhoneNumber,SessionId,LoginTimeUtc,LastActivityUtc,LastHeartbeatUtc,LogoutTimeUtc,DurationSeconds,Duration,Status,IPAddress,UserAgent");

            foreach (var x in rows)
            {
                var end =
                    x.session.TerminatedAtUtc
                    ?? now;

                var duration =
                    end >
                    x.session.CreatedAtUtc
                        ? end -
                          x.session.CreatedAtUtc
                        : TimeSpan.Zero;

                var firstName =
                    x.user.FirstName?.Trim();

                var lastName =
                    x.user.LastName?.Trim();

                var fullName =
                    $"{firstName} {lastName}"
                        .Trim();

                if (string.IsNullOrWhiteSpace(
                    fullName))
                {
                    fullName =
                        x.user.Username;
                }

                csv.AppendLine(
                    string.Join(
                        ",",

                        Csv(
                            x.user.UserId),

                        Csv(
                            fullName),

                        Csv(
                            firstName),

                        Csv(
                            lastName),

                        Csv(
                            x.user.Username),

                        Csv(
                            x.user.Email),

                        Csv(
                            x.user.PhoneNumber),

                        Csv(
                            x.session.SessionId),

                        Csv(
                            x.session.CreatedAtUtc),

                        Csv(
                            x.session.LastActivityUtc),

                        Csv(
                            x.session.LastHeartbeatUtc),

                        Csv(
                            x.session.TerminatedAtUtc),

                        Csv(
                            (long)
                            duration.TotalSeconds),

                        Csv(
                            FormatDuration(
                                duration)),

                        Csv(
                            x.session.IsActive
                                ? "Active"
                                : "Logged Out"),

                        Csv(
                            x.session.IpAddress),

                        Csv(
                            x.session.UserAgent)
                    ));
            }

            var bytes =
                Encoding.UTF8.GetBytes(
                    csv.ToString());

            return File(
                bytes,
                "text/csv",
                $"user-sessions-{DateTime.UtcNow:yyyyMMdd-HHmmss}.csv");
        }


        // =====================================================
        // HELPERS
        // =====================================================

        private static string FormatDuration(
            TimeSpan duration)
        {
            if (duration.TotalSeconds < 0)
                return "0s";

            if (duration.TotalDays >= 1)
            {
                return
                    $"{(int)duration.TotalDays}d " +
                    $"{duration.Hours}h " +
                    $"{duration.Minutes}m";
            }

            if (duration.TotalHours >= 1)
            {
                return
                    $"{(int)duration.TotalHours}h " +
                    $"{duration.Minutes}m";
            }

            if (duration.TotalMinutes >= 1)
            {
                return
                    $"{duration.Minutes}m " +
                    $"{duration.Seconds}s";
            }

            return
                $"{duration.Seconds}s";
        }


        private static string Csv(
            object? value)
        {
            if (value == null)
                return "";

            var text =
                value.ToString() ?? "";

            text =
                text.Replace(
                    "\"",
                    "\"\"");

            return $"\"{text}\"";
        }
    }
}