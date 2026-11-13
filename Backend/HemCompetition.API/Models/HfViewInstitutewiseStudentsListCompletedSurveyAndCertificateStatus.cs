using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfViewInstitutewiseStudentsListCompletedSurveyAndCertificateStatus
{
    public int? StudentId { get; set; }

    public string StudentName { get; set; } = null!;

    public string? Standard { get; set; }

    public int InstituteId { get; set; }

    public string InstituteName { get; set; } = null!;

    public string? MainCourseName { get; set; }

    public string? MobileNo { get; set; }

    public string? EmailId { get; set; }

    public string? CityName { get; set; }

    public bool? EligibleForCertificate { get; set; }

    public int? Certified { get; set; }
}
