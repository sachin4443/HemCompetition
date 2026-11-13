using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfTblTempAffiliateFocusedList
{
    public int Id { get; set; }

    public int? InstituteId { get; set; }

    public string? AffiliateCode { get; set; }
}
