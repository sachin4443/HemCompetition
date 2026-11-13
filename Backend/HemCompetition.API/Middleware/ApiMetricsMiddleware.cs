using System.Diagnostics;
using HemCompetition.API.Diagnostics;
using HemCompetition.API.Services;

namespace HemCompetition.API.Middleware
{
    public class ApiMetricsMiddleware
    {
        private readonly RequestDelegate _next;

        public ApiMetricsMiddleware(
            RequestDelegate next)
        {
            _next = next;
        }


        public async Task InvokeAsync(
            HttpContext context,
            IApiMetricsService metricsService,
            IApiDiagnosticsService diagnosticsService)
        {
            var path =
                context.Request.Path.Value ??
                string.Empty;


            if (!path.StartsWith(
                    "/api/",
                    StringComparison.OrdinalIgnoreCase))
            {
                await _next(context);
                return;
            }


            if (
                path.Equals(
                    "/api/auth/heartbeat",
                    StringComparison.OrdinalIgnoreCase)
            )
            {
                await _next(context);
                return;
            }


            // Existing dashboard metrics endpoint
            if (
                path.Equals(
                    "/api/admin-dashboard/stats",
                    StringComparison.OrdinalIgnoreCase)
            )
            {
                await _next(context);
                return;
            }


            // New diagnostics endpoint itself
            if (
                path.StartsWith(
                    "/api/admin-dashboard/diagnostics",
                    StringComparison.OrdinalIgnoreCase)
            )
            {
                await _next(context);
                return;
            }


            var stopwatch =
                Stopwatch.StartNew();


            var traceId =
                context.TraceIdentifier;


            ApiDiagnosticContext.Set(
                traceId,
                path);


            try
            {
                await _next(context);

                stopwatch.Stop();


                var duration =
                    stopwatch.ElapsedMilliseconds;


                metricsService.RecordRequest(
                    path,
                    context.Request.Method,
                    context.Response.StatusCode,
                    duration);


                diagnosticsService.RecordRequest(
                    path,
                    context.Request.Method,
                    context.Response.StatusCode,
                    duration,
                    null,
                    null,
                    traceId);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();


                var duration =
                    stopwatch.ElapsedMilliseconds;


                metricsService.RecordRequest(
                    path,
                    context.Request.Method,
                    500,
                    duration,
                    ex.Message);


                diagnosticsService.RecordRequest(
                    path,
                    context.Request.Method,
                    500,
                    duration,
                    ex.Message,
                    ex.GetType().FullName,
                    traceId);


                throw;
            }
            finally
            {
                ApiDiagnosticContext.Clear();
            }
        }
    }
}