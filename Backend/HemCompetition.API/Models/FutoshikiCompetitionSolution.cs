using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class FutoshikiCompetitionSolution
{
    public int Fcsid { get; set; }

    public int? Fcmid { get; set; }

    public string? S1 { get; set; }

    public string? S2 { get; set; }

    public string? S3 { get; set; }

    public string? S4 { get; set; }

    public virtual FutoshikiCompetitionMaster? Fcm { get; set; }
}
