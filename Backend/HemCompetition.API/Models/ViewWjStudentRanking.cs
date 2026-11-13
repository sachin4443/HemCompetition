using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class ViewWjStudentRanking
{
    public int StudentId { get; set; }

    public string Standard { get; set; } = null!;

    public string? StudentLevel { get; set; }

    public int? Coins { get; set; }

    public long? Rank { get; set; }

    public long? LevelRank { get; set; }

    public long? SChoolRank { get; set; }

    public int Cups { get; set; }
}
