using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class WjStudentFeedback
{
    public int FeedbackId { get; set; }

    public int StudentId { get; set; }

    public int QuestionId { get; set; }

    public string? Comments { get; set; }

    public DateTime CreateOn { get; set; }

    public virtual User Student { get; set; } = null!;
}
