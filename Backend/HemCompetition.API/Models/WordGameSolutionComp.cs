using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class WordGameSolutionComp
{
    public int Wgsid { get; set; }

    public int? Wgid { get; set; }

    public DateTime? EntryDate { get; set; }

    public string? UserAnswer { get; set; }

    public int? IsCreatedBy { get; set; }

    public string? Remark { get; set; }

    public string? StartTime { get; set; }

    public string? EndTime { get; set; }

    public string? TimeDiff { get; set; }

    public int? PracticalId { get; set; }

    public int? WgtopicId { get; set; }

    public string? GameNames { get; set; }
}
