using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class WjReferralCode
{
    public int ReferralId { get; set; }

    public string? ReferralCode { get; set; }

    public decimal RegistrationAmount { get; set; }

    public decimal CommissionAmount { get; set; }

    public decimal FreeStudentCommision { get; set; }

    public string? TrailType { get; set; }

    public int? TrailDays { get; set; }

    public DateTime ApplicableFrom { get; set; }

    public DateTime? ApplicableTill { get; set; }

    public bool? IsActive { get; set; }

    public int? UserId { get; set; }

    public decimal? CommisssionPerAffiliate { get; set; }

    public decimal? CreditCommission { get; set; }

    public virtual User? User { get; set; }
}
