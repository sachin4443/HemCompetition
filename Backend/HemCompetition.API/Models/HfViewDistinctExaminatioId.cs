using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfViewDistinctExaminatioId
{
    public int UserId { get; set; }

    public int QuizId { get; set; }

    public int? ExamTopicId { get; set; }

    public int? ExaminationId { get; set; }

    public int? RepeatedCount { get; set; }
}
