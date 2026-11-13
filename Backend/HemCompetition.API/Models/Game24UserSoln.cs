using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class Game24UserSoln
{
    public int AnsId { get; set; }

    public int? UserId { get; set; }

    public int? Gcid { get; set; }

    public string? Remark { get; set; }

    public string? UserAnswer { get; set; }

    public string? StartTime { get; set; }

    public string? SubmitTime { get; set; }

    public string? TimeDiff { get; set; }

    public virtual Game24Competition? Gc { get; set; }
}
