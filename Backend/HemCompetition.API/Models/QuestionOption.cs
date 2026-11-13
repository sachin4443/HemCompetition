using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class QuestionOption
{
    public int OptionId { get; set; }

    public int? QuestionId { get; set; }

    public string? Options { get; set; }

    public bool? IsAnswer { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }
}
