namespace HemCompetition.API.DTOs
{
    public class ApiDiagnosticsSummaryDto
    {
        public DateTime WindowStartUtc { get; set; }
        public DateTime WindowEndUtc { get; set; }

        public long TotalRequests { get; set; }
        public long SuccessfulRequests { get; set; }
        public long FailedRequests { get; set; }

        public double RequestsPerSecond { get; set; }

        public double AverageLatencyMs { get; set; }
        public double P50LatencyMs { get; set; }
        public double P95LatencyMs { get; set; }
        public double P99LatencyMs { get; set; }
        public double MaxLatencyMs { get; set; }

        public double ErrorRate { get; set; }

        public SystemDiagnosticsDto System { get; set; } = new();

        public List<ApiEndpointDiagnosticsDto> Endpoints { get; set; } = new();

        public List<ApiDiagnosticRequestDto> RecentRequests { get; set; } = new();

        public List<ApiDiagnosticRequestDto> SlowRequests { get; set; } = new();

        public List<SqlDiagnosticDto> SlowSqlQueries { get; set; } = new();

        public List<string> Bottlenecks { get; set; } = new();
    }


    public class ApiEndpointDiagnosticsDto
    {
        public string Method { get; set; } = "";
        public string Path { get; set; } = "";

        public long Requests { get; set; }
        public long Success { get; set; }
        public long Failed { get; set; }

        public double ErrorRate { get; set; }

        public double AverageLatencyMs { get; set; }
        public double P50LatencyMs { get; set; }
        public double P95LatencyMs { get; set; }
        public double P99LatencyMs { get; set; }
        public double MaxLatencyMs { get; set; }
    }


    public class ApiDiagnosticRequestDto
    {
        public long Id { get; set; }

        public DateTime TimestampUtc { get; set; }

        public string Method { get; set; } = "";
        public string Path { get; set; } = "";

        public int StatusCode { get; set; }

        public long DurationMs { get; set; }

        public string? ErrorReason { get; set; }

        public string? ExceptionType { get; set; }

        public string? TraceId { get; set; }

        public int SqlQueryCount { get; set; }

        public long SqlTotalDurationMs { get; set; }

        public long SlowestSqlDurationMs { get; set; }
    }


    public class SqlDiagnosticDto
    {
        public DateTime TimestampUtc { get; set; }

        public long DurationMs { get; set; }

        public string CommandType { get; set; } = "";

        public string Sql { get; set; } = "";

        public string? TraceId { get; set; }

        public string? Path { get; set; }

        public string? Error { get; set; }
    }


    public class SystemDiagnosticsDto
    {
        public double ProcessCpuPercent { get; set; }

        public double WorkingSetMb { get; set; }

        public double GcHeapMb { get; set; }

        public int ThreadCount { get; set; }

        public int ThreadPoolAvailableWorker { get; set; }

        public int ThreadPoolAvailableIo { get; set; }

        public int ThreadPoolMaxWorker { get; set; }

        public int ThreadPoolMaxIo { get; set; }

        public long UptimeSeconds { get; set; }

        public string ProcessName { get; set; } = "";
    }
}