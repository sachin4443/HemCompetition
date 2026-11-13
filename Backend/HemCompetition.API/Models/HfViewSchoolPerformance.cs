using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfViewSchoolPerformance
{
    public int InstituteId { get; set; }

    public string? Name { get; set; }

    public int? TotalStudents { get; set; }

    public int? ActiveStudents { get; set; }

    public string? ActiveStudentPercentage { get; set; }

    public int? EligibleForCertifcateStudents { get; set; }

    public string? EligibleForCertificateStudentPercentage { get; set; }

    public int? TotalTeachers { get; set; }

    public int? ActiveTeachers { get; set; }

    public string? ActiveTeacherPercentage { get; set; }

    public int? EligibleForCertifcateTeachers { get; set; }

    public string? EligibleForCertificateTeacherPercentage { get; set; }

    public string TotalValues { get; set; } = null!;

    public int? GroupSubmissionCount { get; set; }

    public string? GroupSubmissionPercentage { get; set; }

    public int? AffiliateId { get; set; }

    public string? AffiliateName { get; set; }
}
