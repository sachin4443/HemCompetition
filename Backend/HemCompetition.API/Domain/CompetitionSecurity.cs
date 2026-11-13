namespace HemCompetition.API.Domain
{
    public class CompetitionSecurity
    {
        public Guid Id { get; set; }

        public long UserId { get; set; }

        public string? ConsentVersion { get; set; }

        public bool TermsAccepted { get; set; }

        public DateTime? TermsAcceptedAtUtc { get; set; }

        public string Status { get; set; } = "ACTIVE";

        public string? ViolationType { get; set; }

        public string? ViolationMessage { get; set; }

        public int ViolationCount { get; set; }

        public DateTime? DisqualifiedAtUtc { get; set; }

        public string? LastCameraStatus { get; set; }

        public int? LastPersonCount { get; set; }

        public DateTime? LastViolationAtUtc { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        public DateTime? UpdatedAtUtc { get; set; }
    }
}