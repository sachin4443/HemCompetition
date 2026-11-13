using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfUsersAuth
{
    public int UserId { get; set; }

    public string? EmailId { get; set; }

    public bool EmailConfirmed { get; set; }

    public string? PasswordHash { get; set; }

    public string? SecurityStamp { get; set; }

    public string? PhoneNumber { get; set; }

    public bool PhoneNumberConfirmed { get; set; }

    public string UserName { get; set; } = null!;

    public bool TwoFactorEnabled { get; set; }

    public DateTime? LockoutEndDateUtc { get; set; }

    public bool LockoutEnabled { get; set; }

    public int AccessFailedCount { get; set; }

    public int? RecStatus { get; set; }

    public string? Salt { get; set; }
}
