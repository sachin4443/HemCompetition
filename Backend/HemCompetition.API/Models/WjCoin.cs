using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class WjCoin
{
    public int CoinId { get; set; }

    public int StudentId { get; set; }

    public string? CoinFor { get; set; }

    public int? ReferenceId { get; set; }

    public int? Coins { get; set; }

    public DateTime? CreatedOn { get; set; }

    public int? QuizLevelId { get; set; }

    public int? ExamTopicId { get; set; }
}
