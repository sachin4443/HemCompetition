using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class ViewWjAffiliateCommissionChart
{
    public int AffiliateId { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? InstituteName { get; set; }

    public int? TotalStudents { get; set; }

    public int? DirectStudents { get; set; }

    public int? SchoolStudents { get; set; }

    public int? PaidStudents { get; set; }

    public int? UnPaidStudents { get; set; }

    public decimal? TotalCommissionAmount { get; set; }

    public decimal? SchoolCommissionAmount { get; set; }
}
