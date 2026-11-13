namespace HemCompetition.API.DTOs
{
    public class ApiTrafficBucketDto
    {
        public DateTime TimestampUtc { get; set; }

        public int TotalHits { get; set; }

        public int SuccessfulHits { get; set; }

        public int FailedHits { get; set; }

        public double AverageLatencyMs { get; set; }

        public long MaxLatencyMs { get; set; }
    }
}