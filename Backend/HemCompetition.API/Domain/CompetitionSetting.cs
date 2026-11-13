namespace HemCompetition.API.Domain
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
        public string? Description { get; set; }
        /// <summary>Whether students may see their own scores and the leaderboard.</summary>
        public bool ShowResultsToStudents { get; set; }
        public bool ShowLeaderboard { get; set; }
        public bool ProctoringRequired { get; set; } = true;
    }
}