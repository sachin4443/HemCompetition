using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfViewTeacherWisePerformanceSummary
{
    public int TeacherId { get; set; }

    public string? TeacherName { get; set; }

    public string? TeacherAssignedStandards { get; set; }

    public int Instituteid { get; set; }

    public string? InstituteName { get; set; }

    public int TotalRegisteredStudents { get; set; }

    public int ActiveStudents { get; set; }

    public int StudentsCompletedValue { get; set; }

    public int StudentsCompleted5Values { get; set; }

    public int TeacherCompletedCourses { get; set; }
}
