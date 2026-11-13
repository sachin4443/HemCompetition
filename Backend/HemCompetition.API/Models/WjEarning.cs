using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class WjEarning
{
    public int EarningId { get; set; }

    public int? AffiliateId { get; set; }

    public int? ReceivedStudent { get; set; }

    public int? Earnings { get; set; }

    public string? EarningMode { get; set; }

    public DateTime? CreatedOn { get; set; }
}
