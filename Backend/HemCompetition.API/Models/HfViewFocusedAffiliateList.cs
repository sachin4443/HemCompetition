using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfViewFocusedAffiliateList
{
    public int InstituteId { get; set; }

    public string? InstituteName { get; set; }

    public int? TotalStudents { get; set; }

    public int ActiveStudents { get; set; }

    public int CertifiedStudents { get; set; }

    public int StudentsCompletedCourses { get; set; }

    public int Survey { get; set; }

    public int FamilyPbl { get; set; }

    public int IndividualProject { get; set; }

    public int GroupPbl { get; set; }

    public int TotalTeacher { get; set; }

    public int ActiveTeacher { get; set; }

    public int CertifiedTeacherCount { get; set; }

    public string ReferralCode { get; set; } = null!;

    public string AffiliateName { get; set; } = null!;
}
