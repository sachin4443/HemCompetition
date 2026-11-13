using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class KenCompCage
{
    public int Kccid { get; set; }

    public int Kcmid { get; set; }

    public int? Kctid { get; set; }

    public string? Cage1 { get; set; }

    public string? Cage2 { get; set; }

    public string? Cage3 { get; set; }

    public string? Cage4 { get; set; }

    public string? Cage5 { get; set; }

    public virtual KenCompMaster Kcm { get; set; } = null!;

    public virtual KenCompTarget? Kct { get; set; }
}
