using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class PuzzleCompTransaction
{
    public int PztranId { get; set; }

    public string? IsResolved { get; set; }

    public int? IsResolvedBy { get; set; }

    public DateTime? IsResolvedDate { get; set; }

    public string? Remark { get; set; }

    public string? StartTime { get; set; }

    public string? SubmitTime { get; set; }

    public string? TimeDiff { get; set; }

    public int? CourseId { get; set; }

    public string? GameNames { get; set; }

    public int? UserAnswer { get; set; }
}
