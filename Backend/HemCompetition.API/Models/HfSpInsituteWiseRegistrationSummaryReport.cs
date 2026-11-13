using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfSpInsituteWiseRegistrationSummaryReport
{
    public int AffiliateId { get; set; }

    public string AffiliateName { get; set; } = null!;

    public int InstituteId { get; set; }

    public string? Name { get; set; }

    public string RegisteredStandard { get; set; } = null!;

    public string UnregisteredStandard { get; set; } = null!;

    public string HigherStandardProvider { get; set; } = null!;

    public int TotalStudentCount { get; set; }

    public int ActiveStudentCount { get; set; }

    public int TotalTeacherCount { get; set; }
}
