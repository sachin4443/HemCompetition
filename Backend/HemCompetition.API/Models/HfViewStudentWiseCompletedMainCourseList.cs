using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfViewStudentWiseCompletedMainCourseList
{
    public int? StudentId { get; set; }

    public string StudentName { get; set; } = null!;

    public int? InstituteId { get; set; }

    public string? InstituteName { get; set; }

    public string? Standard { get; set; }

    public int? NoOfCoursesCompleted { get; set; }

    public int NoOfCoursesPending { get; set; }

    public string? CoursesCompleted { get; set; }

    public string CoursesPending { get; set; } = null!;

    public string TotalMarks { get; set; } = null!;

    public int? MarksObtain { get; set; }
}
