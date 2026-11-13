using System;

namespace HemCompetition.API.Models
{
    public class AdminUser
    {
        public Guid Id { get; set; }

        public string Username { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public string? DisplayName { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAtUtc { get; set; }

        public DateTime? UpdatedAtUtc { get; set; }

        public DateTime? LastLoginAtUtc { get; set; }

        public int AccessFailedCount { get; set; } = 0;
    }
}