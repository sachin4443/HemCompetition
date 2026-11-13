using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class ActualExaminationResult
{
    public int UserId { get; set; }

    public int QuizId { get; set; }

    public int? ExamTopicId { get; set; }

    public int? MarksObtain { get; set; }
}
