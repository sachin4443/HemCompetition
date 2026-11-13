using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class WjSessionTracking
{
    public int Id { get; set; }

    public string? WjsessionId { get; set; }

    public string Ipaddress { get; set; } = null!;

    public int? UserId { get; set; }

    public string? Urlaccessed { get; set; }

    public DateTime? TimeAccessed { get; set; }

    public DateTime? CreatedOn { get; set; }
}
