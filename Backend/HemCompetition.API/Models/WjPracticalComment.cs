using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class WjPracticalComment
{
    public int PracticalCommentId { get; set; }

    public int? SubmissionId { get; set; }

    public int? UserId { get; set; }

    public string? Comments { get; set; }

    public DateTime? CreatedOn { get; set; }

    public int? CreatedBy { get; set; }

    public virtual WjPracticalSubmission? Submission { get; set; }
}
