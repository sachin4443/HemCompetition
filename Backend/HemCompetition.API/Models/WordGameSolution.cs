using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class WordGameSolution
{
    public int Wgsid { get; set; }

    public int? Wgid { get; set; }

    public DateTime? EntryDate { get; set; }

    public string? SolutionAnswers { get; set; }

    public int? IsCreatedBy { get; set; }

    public int? CourseId { get; set; }
}
