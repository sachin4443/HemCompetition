using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class VScoreFutoshikiCal
{
    public int AnsId { get; set; }

    public int? Fcid { get; set; }

    public int? UserId { get; set; }

    public int? UserAnswer { get; set; }

    public string? Remark { get; set; }

    public string? StartTime { get; set; }

    public string? EndTime { get; set; }

    public string? TimeDiff { get; set; }

    public string? FutushikiScorePoint { get; set; }
}
