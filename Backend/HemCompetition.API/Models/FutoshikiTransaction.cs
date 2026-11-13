using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class FutoshikiTransaction
{
    public int Ftid { get; set; }

    public int Fid { get; set; }

    public string? Ftarget1 { get; set; }

    public string? Ftarget2 { get; set; }

    public string? Ftarget3 { get; set; }

    public string? Ftarget4 { get; set; }

    public string? Ftarget5 { get; set; }

    public string? Ftarget6 { get; set; }

    public string? Ftarget7 { get; set; }

    public virtual FutoshikiMaster FidNavigation { get; set; } = null!;
}
