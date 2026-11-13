namespace HemCompetition.API.DTOs
{
    public class AdminDashboardSummaryDto
    {
        public long TotalUsers { get; set; }

        public long CurrentLoginUsers { get; set; }

        public long TotalUnloggedUsers { get; set; }

        public long TotalDisqualifiedUsers { get; set; }

        public long TotalActiveSessions { get; set; }

        public DateTime ServerTimeUtc { get; set; }
    }
}