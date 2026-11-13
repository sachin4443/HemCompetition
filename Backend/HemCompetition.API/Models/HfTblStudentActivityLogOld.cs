using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfTblStudentActivityLogOld
{
    public int? StudentId { get; set; }

    public string? StudentName { get; set; }

    public string? Standard { get; set; }

    public int? InstituteId { get; set; }

    public string? InstituteName { get; set; }

    public string? MainCourseName { get; set; }

    public string? CourseName { get; set; }

    public string? TopicName { get; set; }

    public string? ActivityType { get; set; }

    public int? Points { get; set; }

    public DateTime? ActivityDate { get; set; }

    public string? AcedamicYear { get; set; }
}
