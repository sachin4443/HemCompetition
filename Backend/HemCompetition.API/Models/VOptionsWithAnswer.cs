using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class VOptionsWithAnswer
{
    public int? QuestionId { get; set; }

    public string? Option1 { get; set; }

    public string? Option2 { get; set; }

    public string? Option3 { get; set; }

    public string? Option4 { get; set; }

    public string? Option5 { get; set; }

    public string? CorrectAnswers { get; set; }
}
