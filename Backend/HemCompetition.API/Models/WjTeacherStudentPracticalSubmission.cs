using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class WjTeacherStudentPracticalSubmission
{
    public int SubmissionId { get; set; }

    public int StudentId { get; set; }

    public int? PracticalId { get; set; }

    public string? FileName { get; set; }

    public string? FileExtension { get; set; }

    public string? FileLink { get; set; }

    public string? Status { get; set; }

    public string? Remarks { get; set; }

    public DateTime? CreatedDate { get; set; }

    public bool? IsDelete { get; set; }

    public DateTime? IsDeleteDate { get; set; }
}
