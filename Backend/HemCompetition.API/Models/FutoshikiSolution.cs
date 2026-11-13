using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class FutoshikiSolution
{
    public int Fsid { get; set; }

    public int? Fid { get; set; }

    public int? S1 { get; set; }

    public int? S2 { get; set; }

    public int? S3 { get; set; }

    public int? S4 { get; set; }

    public virtual FutoshikiMaster? FidNavigation { get; set; }
}
