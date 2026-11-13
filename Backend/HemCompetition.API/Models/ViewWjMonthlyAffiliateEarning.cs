using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class ViewWjMonthlyAffiliateEarning
{
    public int? AffiliateId { get; set; }

    public string? AffiliateReferralCode { get; set; }

    public decimal? Amount { get; set; }

    public string? PaymentType { get; set; }

    public int? AmountCount { get; set; }

    public decimal? TotalPayment { get; set; }
}
