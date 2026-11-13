using HemCompetition.API.Data;
using HemCompetition.API.Models;

namespace HemCompetition.API.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, AuthDbContext dbContext)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred during execution.");

                var path = context.Request.Path;
                var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
                var userAgent = context.Request.Headers["User-Agent"].ToString();

                var errorLog = new ExceptionLog
                {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace,
                    RequestPath = path,
                    IpAddress = ipAddress,
                    DeviceType = userAgent,
                    Timestamp = DateTime.UtcNow
                };

                dbContext.ExceptionLogs.Add(errorLog);
                await dbContext.SaveChangesAsync();
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;

                var response = new { error = "An internal server error has occurred. The incident has been logged." };
                await context.Response.WriteAsJsonAsync(response);
            }
        }
    }
}
