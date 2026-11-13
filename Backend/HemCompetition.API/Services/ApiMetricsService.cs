using System.Collections.Concurrent;
using HemCompetition.API.DTOs;

namespace HemCompetition.API.Services
{
    public class ApiMetricsService : IApiMetricsService
    {
        // =====================================================
        // GLOBAL COUNTERS
        // =====================================================

        private long _totalHits;
        private long _successfulHits;
        private long _failedHits;
        private long _totalDurationMs;

        // =====================================================
        // RECENT FAILURES
        // =====================================================

        private readonly ConcurrentQueue<ApiMetricLogDto> _recentFailures = new();

        private const int MaxFailureLogs = 50;

        // =====================================================
        // RECENT API REQUESTS
        // RAM ONLY
        // =====================================================

        private readonly ConcurrentQueue<ApiMetricRequestDto> _recentRequests = new();

        private const int MaxRequestLogs = 200;

        private long _requestSequence;

        // =====================================================
        // 10 SECOND TRAFFIC BUCKETS
        // =====================================================

        private readonly ConcurrentDictionary<long, ApiTrafficBucketInternal>
            _trafficBuckets = new();

        private const int BucketSeconds = 10;

        // Keep only last 10 minutes in RAM
        private const int RetentionMinutes = 10;

        // =====================================================
        // RECORD REQUEST
        // =====================================================

        public void RecordRequest(
            string path,
            string method,
            int statusCode,
            long durationMs,
            string? errorReason = null)
        {
            var now = DateTime.UtcNow;

            // =================================================
            // GLOBAL COUNTERS
            // =================================================

            Interlocked.Increment(ref _totalHits);

            Interlocked.Add(
                ref _totalDurationMs,
                durationMs
            );

            if (statusCode >= 200 && statusCode < 400)
            {
                Interlocked.Increment(
                    ref _successfulHits
                );
            }
            else
            {
                Interlocked.Increment(
                    ref _failedHits
                );

                // =============================================
                // FAILURE LOG
                // =============================================

                var failureLog = new ApiMetricLogDto
                {
                    Id = Interlocked.Increment(
                        ref _requestSequence
                    ),

                    Path = path,

                    Method = method,

                    StatusCode = statusCode,

                    DurationMs = durationMs,

                    ErrorReason =
                        errorReason ??
                        GetDefaultReason(statusCode),

                    TimestampUtc = now
                };

                _recentFailures.Enqueue(
                    failureLog
                );

                while (
                    _recentFailures.Count > MaxFailureLogs &&
                    _recentFailures.TryDequeue(out _)
                )
                {
                }
            }

            // =================================================
            // RECENT REQUEST STREAM
            // =================================================

            var requestLog = new ApiMetricRequestDto
            {
                Id = Interlocked.Increment(
                    ref _requestSequence
                ),

                TimestampUtc = now,

                Method = method,

                Path = path,

                StatusCode = statusCode,

                DurationMs = durationMs
            };

            _recentRequests.Enqueue(
                requestLog
            );

            while (
                _recentRequests.Count > MaxRequestLogs &&
                _recentRequests.TryDequeue(out _)
            )
            {
            }

            // =================================================
            // TRAFFIC BUCKET
            // =================================================

            var bucketKey =
                DateTimeOffset.UtcNow.ToUnixTimeSeconds()
                / BucketSeconds;

            var bucket =
                _trafficBuckets.GetOrAdd(
                    bucketKey,
                    _ => new ApiTrafficBucketInternal
                    {
                        TimestampUtc = now
                    }
                );

            Interlocked.Increment(
                ref bucket.TotalHits
            );

            Interlocked.Add(
                ref bucket.TotalDurationMs,
                durationMs
            );

            UpdateMax(
                ref bucket.MaxDurationMs,
                durationMs
            );

            if (
                statusCode >= 200 &&
                statusCode < 400
            )
            {
                Interlocked.Increment(
                    ref bucket.SuccessfulHits
                );
            }
            else
            {
                Interlocked.Increment(
                    ref bucket.FailedHits
                );
            }

            // =================================================
            // CLEAN OLD RAM DATA
            // =================================================

            CleanupOldBuckets();
        }

        // =====================================================
        // GET SUMMARY
        // =====================================================

        public ApiMetricsSummaryDto GetSummary()
        {
            var total =
                Interlocked.Read(
                    ref _totalHits
                );

            var duration =
                Interlocked.Read(
                    ref _totalDurationMs
                );

            // =================================================
            // TRAFFIC HISTORY
            // =================================================

            var history =
                _trafficBuckets
                    .OrderBy(x => x.Key)
                    .Select(x =>
                    {
                        var bucket = x.Value;

                        var hits =
                            Interlocked.Read(
                                ref bucket.TotalHits
                            );

                        var totalDuration =
                            Interlocked.Read(
                                ref bucket.TotalDurationMs
                            );

                        return new ApiTrafficBucketDto
                        {
                            TimestampUtc =
                                bucket.TimestampUtc,

                            TotalHits =
                                (int)hits,

                            SuccessfulHits =
                                (int)Interlocked.Read(
                                    ref bucket.SuccessfulHits
                                ),

                            FailedHits =
                                (int)Interlocked.Read(
                                    ref bucket.FailedHits
                                ),

                            AverageLatencyMs =
                                hits > 0
                                    ? Math.Round(
                                        (double)totalDuration /
                                        hits,
                                        2
                                    )
                                    : 0,

                            MaxLatencyMs =
                                Interlocked.Read(
                                    ref bucket.MaxDurationMs
                                )
                        };
                    })
                    .ToList();

            // =================================================
            // RECENT REQUESTS
            // =================================================

            var recentRequests =
                _recentRequests
                    .Reverse()
                    .Take(100)
                    .ToList();

            // =================================================
            // RETURN
            // =================================================

            return new ApiMetricsSummaryDto
            {
                TotalHits =
                    total,

                SuccessfulHits =
                    Interlocked.Read(
                        ref _successfulHits
                    ),

                FailedHits =
                    Interlocked.Read(
                        ref _failedHits
                    ),

                AverageLatencyMs =
                    total > 0
                        ? Math.Round(
                            (double)duration /
                            total,
                            2
                        )
                        : 0,

                RecentFailures =
                    _recentFailures
                        .Reverse()
                        .Take(20)
                        .ToList(),

                TrafficHistory =
                    history,

                RecentRequests =
                    recentRequests
            };
        }

        // =====================================================
        // CLEAN OLD BUCKETS
        // =====================================================

        private void CleanupOldBuckets()
        {
            var cutoff =
                DateTimeOffset.UtcNow.ToUnixTimeSeconds()
                - (RetentionMinutes * 60);

            foreach (
                var key in _trafficBuckets.Keys
            )
            {
                var bucketTime =
                    key * BucketSeconds;

                if (bucketTime < cutoff)
                {
                    _trafficBuckets.TryRemove(
                        key,
                        out _
                    );
                }
            }
        }

        // =====================================================
        // THREAD SAFE MAX
        // =====================================================

        private static void UpdateMax(
            ref long location,
            long value)
        {
            long current;

            do
            {
                current =
                    Interlocked.Read(
                        ref location
                    );

                if (value <= current)
                {
                    return;
                }

            } while (
                Interlocked.CompareExchange(
                    ref location,
                    value,
                    current
                ) != current
            );
        }

        // =====================================================
        // DEFAULT ERROR
        // =====================================================

        private static string GetDefaultReason(
            int statusCode)
        {
            return statusCode switch
            {
                400 =>
                    "Bad Request / Validation Failed",

                401 =>
                    "Unauthorized / Token Expired / Invalid Credentials",

                403 =>
                    "Forbidden / Access Denied",

                404 =>
                    "Endpoint Not Found",

                408 =>
                    "Request Timeout",

                409 =>
                    "Conflict / Duplicate Request",

                422 =>
                    "Validation Failed",

                429 =>
                    "Too Many Requests",

                500 =>
                    "Internal Server Error",

                502 =>
                    "Bad Gateway",

                503 =>
                    "Service Unavailable",

                504 =>
                    "Gateway Timeout",

                _ =>
                    $"HTTP Error {statusCode}"
            };
        }

        // =====================================================
        // INTERNAL RAM BUCKET
        // =====================================================

        private sealed class ApiTrafficBucketInternal
        {
            public DateTime TimestampUtc { get; set; }

            public long TotalHits;

            public long SuccessfulHits;

            public long FailedHits;

            public long TotalDurationMs;

            public long MaxDurationMs;
        }
    }
}