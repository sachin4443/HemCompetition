namespace HemCompetition.API.Models
{
    public class CompetitionSetting
    {
        public Guid Id { get; set; }

        public string CompetitionName { get; set; } = string.Empty;

        public DateTime StartDateTimeUtc { get; set; }

        public DateTime EndDateTimeUtc { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        public DateTime? UpdatedAtUtc { get; set; }
    }
}