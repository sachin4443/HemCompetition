using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class VQuestionOption
{
    public int? QuestionId { get; set; }

    public string? Options { get; set; }
}
