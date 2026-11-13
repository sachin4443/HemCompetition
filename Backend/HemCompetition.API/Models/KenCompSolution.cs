using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class KenCompSolution
{
    public int Kcsid { get; set; }

    public int? Kcmid { get; set; }

    public string? S1 { get; set; }

    public string? S2 { get; set; }

    public string? S3 { get; set; }

    public string? S4 { get; set; }

    public string? S5 { get; set; }

    public virtual KenCompMaster? Kcm { get; set; }
}
