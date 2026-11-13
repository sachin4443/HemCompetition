using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfTblInstitute
{
    public int InstituteId { get; set; }

    public string? InstituteName { get; set; }

    public string? InstituteType { get; set; }

    public bool? IsVerify { get; set; }

    public int? ReferralId { get; set; }

    public int? EntId { get; set; }

    public DateTime? EntDate { get; set; }

    public int? UpdId { get; set; }

    public DateTime? UpdDate { get; set; }

    public int? RecStatus { get; set; }
}
