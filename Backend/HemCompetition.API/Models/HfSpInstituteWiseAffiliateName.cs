using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfSpInstituteWiseAffiliateName
{
    public int InstituteId { get; set; }

    public string? Name { get; set; }

    public int AffiliateId { get; set; }

    public string AffiliateName { get; set; } = null!;
}
