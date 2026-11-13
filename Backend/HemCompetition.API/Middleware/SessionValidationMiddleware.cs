using HemCompetition.API.Security;
using HemCompetition.API.Services;

namespace HemCompetition.API.Middleware;

/// <summary>
/// Every request made with a student token must belong to that student's one live session.
/// Signing in elsewhere, an admin terminating the session, a suspension, or a missed heartbeat
/// all invalidate the token immediately, regardless of its expiry time.
/// </summary>
public class SessionValidationMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, SessionService sessions)
    {
        var user = context.User;
        if (user.Identity?.IsAuthenticated == true && user.IsStudent())
        {
            var sessionId = user.SessionId();
            if (sessionId == Guid.Empty || !long.TryParse(user.FindFirst("UserId")?.Value, out var userId) ||
                !await sessions.IsValidAsync(userId, sessionId))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new { message = "Your session has ended. Please sign in again.", code = "SESSION_EXPIRED" });
                return;
            }
        }
        await next(context);
    }
}

/// <summary>Baseline browser hardening for every response.</summary>
public class SecurityHeadersMiddleware(RequestDelegate next)
{
    public Task InvokeAsync(HttpContext context)
    {
        var h = context.Response.Headers;
        h["X-Content-Type-Options"] = "nosniff";
        h["X-Frame-Options"] = "DENY";
        h["Referrer-Policy"] = "no-referrer";
        // The camera is needed for proctoring; everything else is switched off.
        h["Permissions-Policy"] = "camera=(self), microphone=(), geolocation=(), payment=()";
        h["Content-Security-Policy"] =
            "default-src 'self'; script-src 'self' 'wasm-unsafe-eval'; style-src 'self' 'unsafe-inline'; " +
            "img-src 'self' data: blob:; media-src 'self' blob:; font-src 'self' data:; connect-src 'self'; " +
            "worker-src 'self' blob:; object-src 'none'; base-uri 'self'; form-action 'self'; frame-ancestors 'none'";
        if (context.Request.Path.StartsWithSegments("/api"))
            h["Cache-Control"] = "no-store";
        return next(context);
    }
}
