using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class WjAffiliateVoucher
{
    public int Id { get; set; }

    public int? FkRequestId { get; set; }

    public DateTime? GeneratedDate { get; set; }

    public string? VoucherCode { get; set; }

    public int? OriginalVoucherCode { get; set; }

    public int? AffiliateId { get; set; }

    public string? AffiliateName { get; set; }

    public string? ReferralCode { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? ValidStartDate { get; set; }

    public DateTime? ValidEndDate { get; set; }

    public bool? Status { get; set; }
}
