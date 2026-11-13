using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfViewCompletedStudentsMainCourseCertificateAsperCertificate
{
    public int? StudentId { get; set; }

    public string StudentName { get; set; } = null!;

    public string? Standard { get; set; }

    public int? InstituteId { get; set; }

    public string? InstituteName { get; set; }

    public string? MainCourseName { get; set; }

    public int TotalMarks { get; set; }

    public int? MarksObtain { get; set; }
}
