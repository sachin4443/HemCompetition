using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfTblUserAdditionApproval
{
    public int ApprovalId { get; set; }

    public int? InstituteId { get; set; }

    public string FileName { get; set; } = null!;

    public string Status { get; set; } = null!;

    public DateTime SubmissionDate { get; set; }

    public int SubmittedByUserId { get; set; }

    public string? UserRole { get; set; }

    public DateTime? ApprovalDate { get; set; }
}
