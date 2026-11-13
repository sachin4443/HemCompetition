using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class VRelUsersAspNetUser
{
    public int UserId { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string UserName { get; set; } = null!;

    public string? Password { get; set; }

    public string? MobileNo { get; set; }

    public string AspNetUserId { get; set; } = null!;

    public string Id { get; set; } = null!;
}
