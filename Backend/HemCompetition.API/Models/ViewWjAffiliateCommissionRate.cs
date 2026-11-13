using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class ViewWjAffiliateCommissionRate
{
    public int StudentId { get; set; }

    public int? AffiliateId { get; set; }

    public string? Source { get; set; }

    public int? SourceId { get; set; }

    public string? ReferralCode { get; set; }

    public decimal RegistrationAmount { get; set; }

    public bool? IsActive { get; set; }

    public bool IsPaid { get; set; }

    public decimal? CommissionAmount { get; set; }
}
