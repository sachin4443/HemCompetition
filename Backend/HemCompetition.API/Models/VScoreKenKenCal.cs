using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class VScoreKenKenCal
{
    public int? UserId { get; set; }

    public int? Kcmid { get; set; }

    public string? TimeDiff { get; set; }

    public string UserAnswer { get; set; } = null!;

    public string? KenKenScorePoint { get; set; }
}
