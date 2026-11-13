using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class ViewExamDistinctResult
{
    public long? Id { get; set; }

    public int ExamId { get; set; }

    public int UserId { get; set; }

    public DateTime? AppearedDate { get; set; }

    public int QuizId { get; set; }

    public string QuizName { get; set; } = null!;

    public int? TotalMarks { get; set; }

    public int? PassMarks { get; set; }

    public int? MarksObtain { get; set; }

    public int? Percentage { get; set; }

    public int? TotalAttempt { get; set; }
}
