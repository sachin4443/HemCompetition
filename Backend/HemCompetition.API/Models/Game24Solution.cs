using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class Game24Solution
{
    public int Sid { get; set; }

    public int? Game24Id { get; set; }

    public DateTime? EntryDate { get; set; }

    public string? SolutionAnswers { get; set; }

    public int? IsCreatedBy { get; set; }

    public int? CourseId { get; set; }
}
