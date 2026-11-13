namespace HemCompetition.API.DTOs
{
    public class ApiMetricLogDto
    {
        public long Id { get; set; }

        public string Path { get; set; } = string.Empty;

        public string Method { get; set; } = string.Empty;

        public int StatusCode { get; set; }

        public long DurationMs { get; set; }

        public string ErrorReason { get; set; } = string.Empty;

        public DateTime TimestampUtc { get; set; }
    }
}