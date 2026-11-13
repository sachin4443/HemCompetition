namespace HemCompetition.API.Domain
{
    public class AuthUser
    {
        public Guid Id { get; set; }

        // MSSQL Users.UserId
        public long UserId { get; set; }

        public string Username { get; set; } = string.Empty;

        public string? Email { get; set; }

        public string PasswordHash { get; set; } = string.Empty;

        public string? SecurityStamp { get; set; }

        public string? PhoneNumber { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? EmailId { get; set; }

        public string? AddressLine1 { get; set; }

        public string? City { get; set; }

        public string? Pincode { get; set; }

        public bool IsActive { get; set; } = true;

        public bool EmailConfirmed { get; set; } = true;

        public bool PhoneNumberConfirmed { get; set; } = false;

        public bool TwoFactorEnabled { get; set; } = false;

        public bool LockoutEnabled { get; set; } = true;

        public DateTime? LockoutEndDateUtc { get; set; }

        public int AccessFailedCount { get; set; }

        public bool IsUserDeactivated { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        public DateTime? UpdatedAtUtc { get; set; }

        public DateTime? LastLoginAtUtc { get; set; }
    }
}