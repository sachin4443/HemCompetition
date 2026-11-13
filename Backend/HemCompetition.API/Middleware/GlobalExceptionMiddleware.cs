using HemCompetition.API.Data;
using HemCompetition.API.Domain;
using HemCompetition.API.Infrastructure;

namespace HemCompetition.API.Middleware;

/// <summary>
/// Turns expected failures into clean error responses and logs unexpected ones.
/// Internal details (messages, stack traces) are stored in the log table, never returned.
/// </summary>
public class GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context, IServiceScopeFactory scopes)
    {
        try
        {
            await next(context);
        }
        catch (ApiException ex)
        {
            if (context.Response.HasStarted) throw;
            context.Response.StatusCode = ex.StatusCode;
            await context.Response.WriteAsJsonAsync(new { message = ex.Message, code = ex.Code });
        }
        catch (UnauthorizedAccessException)
        {
            if (context.Response.HasStarted) throw;
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new { message = "Please sign in again.", code = "UNAUTHORIZED" });
        }
        catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
        {
            // The client went away; nothing to report.
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception for {Method} {Path}", context.Request.Method, context.Request.Path);
            try
            {
                // A fresh scope: the request's own DbContext may be the thing that failed.
                using var scope = scopes.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                db.ExceptionLogs.Add(new ExceptionLog
                {
                    Message = ex.Message,
                    StackTrace = ex.ToString(),
                    RequestPath = context.Request.Path,
                    IpAddress = context.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
                    DeviceType = context.Request.Headers.UserAgent.ToString(),
                    Timestamp = DateTime.UtcNow
                });
                await db.SaveChangesAsync();
            }
            catch (Exception logEx)
            {
                logger.LogError(logEx, "Could not store the exception log entry.");
            }
            if (context.Response.HasStarted) throw;
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(new { message = "Something went wrong. The incident has been logged.", code = "SERVER_ERROR" });
        }
    }
}
