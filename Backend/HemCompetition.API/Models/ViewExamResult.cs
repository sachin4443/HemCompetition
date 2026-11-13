using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class ViewExamResult
{
    public long RoWno { get; set; }

    public int? RecordId { get; set; }

    public int? ExaminationId { get; set; }

    public string? Question { get; set; }

    public string? YourAnswer { get; set; }

    public string? CorrectAnswer { get; set; }
}
