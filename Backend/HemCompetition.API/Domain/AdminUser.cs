using System;

namespace HemCompetition.API.Domain
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
        public string? Email { get; set; }
        public int? RoleId { get; set; }
        public DateTime? LockoutEndUtc { get; set; }
        /// <summary>Forces a password change at next login (set for accounts created with a temporary password).</summary>
        public bool MustChangePassword { get; set; }
        public AdminRole? Role { get; set; }
    }
}