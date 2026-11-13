using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class WjStudentAffiliateAssociate
{
    public int Id { get; set; }

    public int StudentId { get; set; }

    public int? AffiliateId { get; set; }

    public string? ReferralCode { get; set; }

    public DateTime? Createdon { get; set; }
}
