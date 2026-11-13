using HemCompetition.API.Services;

namespace HemCompetition.API.Middleware
{
    public class SessionValidationMiddleware
    {
        private readonly RequestDelegate _next;

        public SessionValidationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(
            HttpContext context,
            IAuthSessionService authSessionService)
        {
            // ==========================================
            // ADMIN JWT
            // ==========================================

            if (context.User.Identity?.IsAuthenticated == true &&
                context.User.HasClaim("AdminRole", "Admin"))
            {
                await _next(context);
                return;
            }


            var path = context.Request.Path.Value?.ToLowerInvariant();


            // ==========================================
            // PUBLIC APIs / STATIC FILES
            // ==========================================

            if (path != null &&
                (
                    path.StartsWith("/api/auth/login") ||
                    path.StartsWith("/api/auth/terminate-session") ||
                    path.StartsWith("/api/auth/logout") ||
                    path.StartsWith("/api/auth/heartbeat") ||
                    path.StartsWith("/api/adminauth/login") ||
                    path.StartsWith("/swagger") ||
                    path.StartsWith("/favicon") ||
                    path.StartsWith("/index.html") ||
                    path.StartsWith("/assets") ||
                    path.StartsWith("/css") ||
                    path.StartsWith("/js") ||
                    path.StartsWith("/images")
                ))
            {
                await _next(context);
                return;
            }


            // ==========================================
            // NORMAL USER SESSION
            // ==========================================

            if (context.User.Identity?.IsAuthenticated == true)
            {
                var userIdClaim =
                    context.User.FindFirst("UserId")?.Value;

                var sessionIdClaim =
                    context.User.FindFirst("SessionId")?.Value;


                if (string.IsNullOrEmpty(userIdClaim) ||
                    string.IsNullOrEmpty(sessionIdClaim))
                {
                    context.Response.StatusCode = 401;

                    await context.Response.WriteAsJsonAsync(new
                    {
                        message = "Invalid Session."
                    });

                    return;
                }


                if (!long.TryParse(
                        userIdClaim,
                        out var userId))
                {
                    context.Response.StatusCode = 401;

                    await context.Response.WriteAsJsonAsync(new
                    {
                        message = "Invalid UserId."
                    });

                    return;
                }


                if (!Guid.TryParse(
                        sessionIdClaim,
                        out var sessionId))
                {
                    context.Response.StatusCode = 401;

                    await context.Response.WriteAsJsonAsync(new
                    {
                        message = "Invalid SessionId."
                    });

                    return;
                }


                var valid =
                    await authSessionService
                        .ValidateSessionAsync(
                            userId,
                            sessionId);


                if (!valid)
                {
                    context.Response.StatusCode = 401;

                    await context.Response.WriteAsJsonAsync(new
                    {
                        message = "Session Expired."
                    });

                    return;
                }
            }


            await _next(context);
        }
    }
}