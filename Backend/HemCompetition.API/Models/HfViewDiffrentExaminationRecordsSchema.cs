using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfViewDiffrentExaminationRecordsSchema
{
    public int ExaminationId { get; set; }

    public int UserId { get; set; }

    public DateTime? StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public int QuizId { get; set; }

    public int? LevelId { get; set; }

    public int? MarksObtain { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public bool IsPassed { get; set; }

    public int? CoinsEarned { get; set; }

    public int? ExamTopicId { get; set; }

    public long? Rn { get; set; }
}
