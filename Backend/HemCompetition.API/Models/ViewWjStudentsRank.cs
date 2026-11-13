using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class ViewWjStudentsRank
{
    public int StudentId { get; set; }

    public int Coins { get; set; }

    public long? Rank { get; set; }
}
