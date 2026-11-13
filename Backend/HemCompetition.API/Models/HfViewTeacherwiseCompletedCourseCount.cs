using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfViewTeacherwiseCompletedCourseCount
{
    public int? TeacherId { get; set; }

    public string TeacherName { get; set; } = null!;

    public int? InstituteId { get; set; }

    public string? InstituteName { get; set; }

    public int? CompletedvalueCount { get; set; }

    public int? TotalMarksObtained { get; set; }
}
