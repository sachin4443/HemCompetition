using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfViewStudentsCourseCompletedList
{
    public int? StudentId { get; set; }

    public string? FirstName { get; set; }

    public int? InstituteId { get; set; }

    public string? InstituteName { get; set; }

    public string? Standard { get; set; }

    public int? NoOfCoursesCompleted { get; set; }

    public int? NoOfCoursesPending { get; set; }

    public string? CoursesCompleted { get; set; }

    public string? CoursesPending { get; set; }
}
