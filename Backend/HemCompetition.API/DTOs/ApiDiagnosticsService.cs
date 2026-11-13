using System.Collections.Concurrent;
using System.Diagnostics;
using HemCompetition.API.DTOs;

namespace HemCompetition.API.Services
{
    public interface IApiDiagnosticsService
    {
        void RecordRequest(
            string path,
            string method,
            int statusCode,
            long durationMs,
            string? errorReason,
            string? exceptionType,
            string? traceId);

        ApiDiagnosticsSummaryDto GetSummary();
    }


    public class ApiDiagnosticsService : IApiDiagnosticsService
    {
        private readonly ISqlDiagnosticsService _sql;

        private readonly ConcurrentQueue<ApiDiagnosticRequestDto> _requests = new();

        private long _sequence;

        private const int MaxRequests = 50000;

        private static readonly TimeSpan Retention =
            TimeSpan.FromMinutes(5);


        public ApiDiagnosticsService(
            ISqlDiagnosticsService sql)
        {
            _sql = sql;
        }


        public void RecordRequest(
            string path,
            string method,
            int statusCode,
            long durationMs,
            string? errorReason,
            string? exceptionType,
            string? traceId)
        {
            var request = new ApiDiagnosticRequestDto
            {
                Id = Interlocked.Increment(ref _sequence),

                TimestampUtc = DateTime.UtcNow,

                Method = method,

                Path = path,

                StatusCode = statusCode,

                DurationMs = durationMs,

                ErrorReason = errorReason,

                ExceptionType = exceptionType,

                TraceId = traceId
            };


            var sqlLogs = _sql.GetForTrace(traceId);

            request.SqlQueryCount = sqlLogs.Count;

            request.SqlTotalDurationMs =
                sqlLogs.Sum(x => x.DurationMs);

            request.SlowestSqlDurationMs =
                sqlLogs.Count > 0
                    ? sqlLogs.Max(x => x.DurationMs)
                    : 0;


            _requests.Enqueue(request);

            Cleanup();
        }


        public ApiDiagnosticsSummaryDto GetSummary()
        {
            var now = DateTime.UtcNow;

            var since = now.Subtract(Retention);

            Cleanup();


            var requests = _requests
                .Where(x => x.TimestampUtc >= since)
                .OrderBy(x => x.TimestampUtc)
                .ToList();


            var durations = requests
                .Select(x => (double)x.DurationMs)
                .OrderBy(x => x)
                .ToList();


            var total = requests.Count;

            var success = requests.Count(x =>
                x.StatusCode >= 200 &&
                x.StatusCode < 400);

            var failed = total - success;


            return new ApiDiagnosticsSummaryDto
            {
                WindowStartUtc = since,

                WindowEndUtc = now,

                TotalRequests = total,

                SuccessfulRequests = success,

                FailedRequests = failed,

                RequestsPerSecond =
                    requests.Count / 300.0,

                AverageLatencyMs =
                    total > 0
                        ? Math.Round(
                            durations.Average(),
                            2)
                        : 0,

                P50LatencyMs =
                    Percentile(durations, 0.50),

                P95LatencyMs =
                    Percentile(durations, 0.95),

                P99LatencyMs =
                    Percentile(durations, 0.99),

                MaxLatencyMs =
                    durations.Count > 0
                        ? durations.Max()
                        : 0,

                ErrorRate =
                    total > 0
                        ? Math.Round(
                            failed * 100.0 / total,
                            2)
                        : 0,

                Endpoints =
                    BuildEndpointStats(requests),

                RecentRequests =
                    requests
                        .OrderByDescending(x => x.TimestampUtc)
                        .ToList(),

                SlowRequests =
                    requests
                        .Where(x => x.DurationMs >= 1000)
                        .OrderByDescending(x => x.DurationMs)
                        .Take(100)
                        .ToList(),

                SlowSqlQueries =
                    _sql.GetRecent(
                        since,
                        100),

                System =
                    GetSystemDiagnostics(),

                Bottlenecks =
                    DetectBottlenecks(
                        requests)
            };
        }


        private List<ApiEndpointDiagnosticsDto>
            BuildEndpointStats(
                List<ApiDiagnosticRequestDto> requests)
        {
            return requests
                .GroupBy(x => new
                {
                    x.Method,
                    x.Path
                })
                .Select(group =>
                {
                    var durations = group
                        .Select(x => (double)x.DurationMs)
                        .OrderBy(x => x)
                        .ToList();

                    var total = group.Count();

                    var failed = group.Count(x =>
                        x.StatusCode >= 400);

                    return new ApiEndpointDiagnosticsDto
                    {
                        Method = group.Key.Method,

                        Path = group.Key.Path,

                        Requests = total,

                        Success = total - failed,

                        Failed = failed,

                        ErrorRate =
                            total > 0
                                ? Math.Round(
                                    failed * 100.0 / total,
                                    2)
                                : 0,

                        AverageLatencyMs =
                            Math.Round(
                                durations.Average(),
                                2),

                        P50LatencyMs =
                            Percentile(
                                durations,
                                .50),

                        P95LatencyMs =
                            Percentile(
                                durations,
                                .95),

                        P99LatencyMs =
                            Percentile(
                                durations,
                                .99),

                        MaxLatencyMs =
                            durations.Max()
                    };
                })
                .OrderByDescending(x => x.P95LatencyMs)
                .ToList();
        }


        private static double Percentile(
            List<double> values,
            double percentile)
        {
            if (values.Count == 0)
                return 0;

            if (values.Count == 1)
                return values[0];

            var position =
                (values.Count - 1) * percentile;

            var lower = (int)Math.Floor(position);

            var upper = (int)Math.Ceiling(position);

            if (lower == upper)
                return Math.Round(values[lower], 2);

            var weight = position - lower;

            return Math.Round(
                values[lower] +
                (values[upper] - values[lower]) *
                weight,
                2);
        }


        private void Cleanup()
        {
            var cutoff =
                DateTime.UtcNow.Subtract(Retention);

            while (
                _requests.TryPeek(
                    out var request) &&
                request.TimestampUtc < cutoff)
            {
                _requests.TryDequeue(
                    out _);
            }

            while (_requests.Count > MaxRequests)
            {
                _requests.TryDequeue(out _);
            }
        }


        private static SystemDiagnosticsDto
            GetSystemDiagnostics()
        {
            var process =
                Process.GetCurrentProcess();

            ThreadPool.GetAvailableThreads(
                out var availableWorker,
                out var availableIo);

            ThreadPool.GetMaxThreads(
                out var maxWorker,
                out var maxIo);


            return new SystemDiagnosticsDto
            {
                ProcessName =
                    process.ProcessName,

                WorkingSetMb =
                    Math.Round(
                        process.WorkingSet64 /
                        1024d /
                        1024d,
                        2),

                GcHeapMb =
                    Math.Round(
                        GC.GetTotalMemory(false) /
                        1024d /
                        1024d,
                        2),

                ThreadCount =
                    process.Threads.Count,

                ThreadPoolAvailableWorker =
                    availableWorker,

                ThreadPoolAvailableIo =
                    availableIo,

                ThreadPoolMaxWorker =
                    maxWorker,

                ThreadPoolMaxIo =
                    maxIo,

                UptimeSeconds =
                    (long)(
                        DateTime.Now -
                        process.StartTime
                    ).TotalSeconds
            };
        }


        private static List<string>
            DetectBottlenecks(
                List<ApiDiagnosticRequestDto> requests)
        {
            var result = new List<string>();

            if (requests.Count == 0)
                return result;


            var p95 =
                Percentile(
                    requests
                        .Select(x =>
                            (double)x.DurationMs)
                        .OrderBy(x => x)
                        .ToList(),
                    .95);


            if (p95 >= 2000)
            {
                result.Add(
                    "API latency bottleneck: P95 is above 2 seconds.");
            }
            else if (p95 >= 1000)
            {
                result.Add(
                    "API latency warning: P95 is above 1 second.");
            }


            var errorRate =
                requests.Count(x =>
                    x.StatusCode >= 500)
                * 100.0 /
                requests.Count;


            if (errorRate >= 5)
            {
                result.Add(
                    "Server error bottleneck: HTTP 5xx rate is above 5%.");
            }


            var sqlSlow =
                requests.Any(x =>
                    x.SlowestSqlDurationMs >= 1000);


            if (sqlSlow)
            {
                result.Add(
                    "Database bottleneck: slow SQL query detected.");
            }


            var sqlHeavy =
                requests.Any(x =>
                    x.SqlQueryCount >= 20);


            if (sqlHeavy)
            {
                result.Add(
                    "Database bottleneck: request is executing a high number of SQL queries.");
            }


            return result;
        }
    }
}