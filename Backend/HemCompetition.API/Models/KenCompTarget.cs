using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class KenCompTarget
{
    public int Kctid { get; set; }

    public int Kcmid { get; set; }

    public string? Target1 { get; set; }

    public string? Target2 { get; set; }

    public string? Target3 { get; set; }

    public string? Target4 { get; set; }

    public string? Target5 { get; set; }

    public virtual KenCompMaster Kcm { get; set; } = null!;

    public virtual ICollection<KenCompCage> KenCompCages { get; set; } = new List<KenCompCage>();
}
