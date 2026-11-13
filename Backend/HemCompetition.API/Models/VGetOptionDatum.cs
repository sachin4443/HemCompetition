using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class VGetOptionDatum
{
    public int? OptionId { get; set; }

    public int? QuestionId { get; set; }

    public string? Options { get; set; }
}
