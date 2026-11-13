using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfViewAffiliatewiseValueCompletedsummary
{
    public string AffiliateName { get; set; } = null!;

    public int? InstituteId { get; set; }

    public string? InstituteName { get; set; }

    public string? Standard { get; set; }

    public string? MainCourseName { get; set; }

    public int TotalRegisteredStudents { get; set; }

    public int? StudentsCompletedSurvey { get; set; }
}
