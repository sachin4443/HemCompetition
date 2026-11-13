using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class AnubhutiTransaction
{
    public int AnbTranId { get; set; }

    public int? AnbId { get; set; }

    public string? Action1 { get; set; }

    public string? Action2 { get; set; }

    public string? Comments1 { get; set; }

    public string? Comments2 { get; set; }

    public string? Comments3 { get; set; }

    public int? IsResolvedBy { get; set; }

    public DateTime? IsResolvedDate { get; set; }

    public virtual AnubhutiMaster? Anb { get; set; }
}
