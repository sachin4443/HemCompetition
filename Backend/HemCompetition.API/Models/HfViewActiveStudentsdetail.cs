using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfViewActiveStudentsdetail
{
    public int? StudentId { get; set; }

    public string? StudentName { get; set; }

    public string? Standard { get; set; }

    public int? InstituteId { get; set; }

    public string? InstituteName { get; set; }

    public string TotalMarks { get; set; } = null!;

    public int? GainMarks { get; set; }

    public string? MainCourseDetailswithPoints { get; set; }
}
