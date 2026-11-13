using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class StudentRankView
{
    public int StudentId { get; set; }

    public int? Coins { get; set; }

    public string Standard { get; set; } = null!;

    public long? Rank { get; set; }
}
