using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfViewVacationProjectSubmission
{
    public int? AffiliateId { get; set; }

    public string AffiliateName { get; set; } = null!;

    public string? SchoolName { get; set; }

    public int? InstituteId { get; set; }

    public string? MainCourseName { get; set; }

    public string? CoOrdinatorName { get; set; }

    public string? CityName { get; set; }

    public string? ReferredBy { get; set; }

    public string? MobileNo { get; set; }

    public string? ImageActivity { get; set; }

    public string? VideoActivity { get; set; }

    public string? ShortDescription { get; set; }

    public DateTime? SubmissionDate { get; set; }
}
