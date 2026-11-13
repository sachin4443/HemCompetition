namespace HemCompetition.API.Domain
{
    public class AuthSession
    {
        public Guid Id { get; set; }

        // MSSQL Users.UserId
        public long UserId { get; set; }

        // PostgreSQL auth_sessions.session_id -> uuid
        public Guid SessionId { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        public DateTime LastActivityUtc { get; set; }

        public DateTime? LastHeartbeatUtc { get; set; }

        public bool IsActive { get; set; }

        public string? IpAddress { get; set; }

        public string? UserAgent { get; set; }

        public DateTime? TerminatedAtUtc { get; set; }
    }
}