namespace HemCompetition.API.Domain
{
    public class ExceptionLog
    {
        public int Id { get; set; }
        public string? Message { get; set; }
        public string? StackTrace { get; set; }
        public string? RequestPath { get; set; }
        public string? DeviceType { get; set; }
        public string? IpAddress { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
