using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfSectionSubmission
{
    public int SubmissionId { get; set; }

    public int? SectionId { get; set; }

    public string? MobileNo { get; set; }

    public DateTime? SubmittedOn { get; set; }
}
