using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfViewTeacherCourseSummary
{
    public int? TeacherId { get; set; }

    public string TeacherName { get; set; } = null!;

    public int? InstituteId { get; set; }

    public string? InstituteName { get; set; }

    public string? CourseCompleted { get; set; }

    public string? CoursePending { get; set; }

    public int? NoOfcourseCompleted { get; set; }

    public int? NoofCoursePending { get; set; }

    public int? CertifiedinMainCourseCount { get; set; }

    public int? TotalObtainedMarks { get; set; }
}
