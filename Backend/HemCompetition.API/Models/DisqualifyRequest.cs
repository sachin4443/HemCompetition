namespace HemCompetition.API.Models
{
    public class DisqualifyRequest
    {
        public string? ViolationType { get; set; }

        public string? ViolationMessage { get; set; }

        public string? CameraStatus { get; set; }

        public int PersonCount { get; set; }
    }
}