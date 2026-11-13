using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class Templist
{
    public int SubmissionId { get; set; }

    public int? MainCourseId { get; set; }

    public string? MainCourseName { get; set; }

    public string? GroupName { get; set; }

    public string? ShortDescription { get; set; }

    public string? ImageActivity { get; set; }

    public string? VideoActivity { get; set; }

    public DateTime? CreatedDate { get; set; }

    public bool IsBest { get; set; }

    public string? StudentList { get; set; }

    public int? AffiliateId { get; set; }

    public string? AffiliateName { get; set; }

    public int? InstituteId { get; set; }

    public string? InstituteName { get; set; }
}
