namespace HemCompetition.API.DTOs
{
    public class ApiMetricRequestDto
    {
        public long Id { get; set; }

        public DateTime TimestampUtc { get; set; }

        public string Method { get; set; } = string.Empty;

        public string Path { get; set; } = string.Empty;

        public int StatusCode { get; set; }

        public long DurationMs { get; set; }
    }
}