namespace HemCompetition.API.DTOs
{
    public class ApiMetricsSummaryDto
    {
        public long TotalHits { get; set; }

        public long SuccessfulHits { get; set; }

        public long FailedHits { get; set; }

        public double AverageLatencyMs { get; set; }

        public List<ApiMetricLogDto> RecentFailures { get; set; } = new();

        public List<ApiTrafficBucketDto> TrafficHistory { get; set; } = new();

        public List<ApiMetricRequestDto> RecentRequests { get; set; } = new();
    }
}