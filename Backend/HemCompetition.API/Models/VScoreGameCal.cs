using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class VScoreGameCal
{
    public int AnsId { get; set; }

    public int? UserId { get; set; }

    public int? Gcid { get; set; }

    public string? Remark { get; set; }

    public string? UserAnswer { get; set; }

    public string? StartTime { get; set; }

    public string? SubmitTime { get; set; }

    public string? TimeDiff { get; set; }

    public string? Game24ScorePoint { get; set; }
}
