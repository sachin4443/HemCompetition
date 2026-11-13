using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfViewStudentActivityLog
{
    public int? StudentId { get; set; }

    public string StudentName { get; set; } = null!;

    public string? Standard { get; set; }

    public int InstituteId { get; set; }

    public string InstituteName { get; set; } = null!;

    public string? MainCourseName { get; set; }

    public string? CourseName { get; set; }

    public string? TopicName { get; set; }

    public string ActivityType { get; set; } = null!;

    public int? Points { get; set; }

    public DateTime? ActivityDate { get; set; }
}
