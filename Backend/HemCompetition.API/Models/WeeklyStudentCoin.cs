using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class WeeklyStudentCoin
{
    public int StudentId { get; set; }

    public int? WeeklyCoins { get; set; }
}
