using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class ViewWjStudentsAdminList
{
    public int StudentId { get; set; }

    public string? Division { get; set; }

    public string? GardianName { get; set; }

    public bool IsPaid { get; set; }

    public string? ReferralCode { get; set; }

    public decimal RegistrationAmount { get; set; }

    public string Standard { get; set; } = null!;

    public bool? IsUserDeactivated { get; set; }

    public int? InstituteId { get; set; }

    public string? InstituteName { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? CityName { get; set; }

    public string? EmailId { get; set; }

    public string? Pincode { get; set; }

    public int? Coins { get; set; }

    public long? Rank { get; set; }

    public string? Gender { get; set; }

    public string? MobileNo { get; set; }

    public DateOnly? RegistrationDate { get; set; }

    public string? RegistrationDisplayDate { get; set; }

    public string? Username { get; set; }

    public string? Password { get; set; }

    public int Credit { get; set; }

    public int EnquiryStatus { get; set; }

    public int IsGoogler { get; set; }
}
