using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfViewMinandMaxExaminatioId
{
    public int QuizId { get; set; }

    public int? ExamTopicId { get; set; }

    public int UserId { get; set; }

    public int? MinExaminationId { get; set; }
}
