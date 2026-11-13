using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class KenCompUserAnswer
{
    public int AnsId { get; set; }

    public int? UserId { get; set; }

    public int? Kcmid { get; set; }

    public string? UserAnswer { get; set; }

    public string? Remark { get; set; }

    public string? StartTime { get; set; }

    public string? EndTime { get; set; }

    public string? TimeDiff { get; set; }

    public int? CourseId { get; set; }
}
