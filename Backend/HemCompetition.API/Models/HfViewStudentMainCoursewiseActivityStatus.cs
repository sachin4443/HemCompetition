using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfViewStudentMainCoursewiseActivityStatus
{
    public int? StudentId { get; set; }

    public string StudentName { get; set; } = null!;

    public int? InstituteId { get; set; }

    public string? InstituteName { get; set; }

    public string? Standard { get; set; }

    public string? MainCourseName { get; set; }

    public string TotalPoints { get; set; } = null!;

    public int? MarksObtain { get; set; }

    public string GroupPblstatus { get; set; } = null!;

    public string IndividualPblstatus { get; set; } = null!;

    public string PostValueSurveyStatus { get; set; } = null!;
}
