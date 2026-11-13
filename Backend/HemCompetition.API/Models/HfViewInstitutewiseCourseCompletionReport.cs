using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfViewInstitutewiseCourseCompletionReport
{
    public int StudentId { get; set; }

    public string? StudentName { get; set; }

    public string Standard { get; set; } = null!;

    public string? InstituteName { get; set; }

    public int? InstituteId { get; set; }

    public int? MainCourseId { get; set; }

    public string? MainCourseName { get; set; }

    public int? TotalCourses { get; set; }

    public int? CompletedCourse { get; set; }

    public int? PendingCourse { get; set; }

    public string? CompletedCourseName { get; set; }

    public string? PendingCourseName { get; set; }
}
