using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class VScoreScrambleCal
{
    public int AnsId { get; set; }

    public int? QuestionId { get; set; }

    public int? ScrambleTopicId { get; set; }

    public string? Answer { get; set; }

    public int? UserId { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? Remark { get; set; }

    public string? StartTime { get; set; }

    public string? SubmitTime { get; set; }

    public string? TimeDiff { get; set; }

    public int? PracticalId { get; set; }

    public string? GameNames { get; set; }

    public string? ScrambleScorePoint { get; set; }
}
