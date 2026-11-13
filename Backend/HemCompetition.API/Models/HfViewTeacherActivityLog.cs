using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfViewTeacherActivityLog
{
    public int? TeacherId { get; set; }

    public string TeacherName { get; set; } = null!;

    public string? Standard { get; set; }

    public int? InstituteId { get; set; }

    public string? InstituteName { get; set; }

    public string? MainCourseName { get; set; }

    public string? CourseName { get; set; }

    public string? TopicName { get; set; }

    public string ActivityType { get; set; } = null!;

    public int? Points { get; set; }

    public DateTime? ActivityDate { get; set; }
}
