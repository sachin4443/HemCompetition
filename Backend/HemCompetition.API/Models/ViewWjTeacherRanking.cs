using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class ViewWjTeacherRanking
{
    public int TeacherId { get; set; }

    public int Coins { get; set; }

    public long? Rank { get; set; }

    public long? SchoolRank { get; set; }
}
