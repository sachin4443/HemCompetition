using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class WjEventUser
{
    public int EventUserId { get; set; }

    public int FkeventId { get; set; }

    public int UserId { get; set; }

    public bool IsActive { get; set; }

    public string? Message { get; set; }

    public DateTime? CreatedOn { get; set; }

    public string? Role { get; set; }

    public DateTime? ExpiredOn { get; set; }
}
