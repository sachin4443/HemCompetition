using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class ExaminationRecord
{
    public int RecordId { get; set; }

    public int? ExaminationId { get; set; }

    public int? QuestionId { get; set; }

    public string? RecordStatus { get; set; }

    public int? Answer { get; set; }

    public bool? IsCorrect { get; set; }

    public DateTime? SubmitTiming { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? OrderNo { get; set; }
}
