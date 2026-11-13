using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class FutoshikiCompetitionTarget
{
    public int Fctid { get; set; }

    public int Fcmid { get; set; }

    public string? Target1 { get; set; }

    public string? Target2 { get; set; }

    public string? Target3 { get; set; }

    public string? Target4 { get; set; }

    public string? Target5 { get; set; }

    public string? Target6 { get; set; }


    public virtual FutoshikiCompetitionMaster Fcm { get; set; } = null!;
}
