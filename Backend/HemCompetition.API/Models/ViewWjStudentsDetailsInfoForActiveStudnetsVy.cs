using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class ViewWjStudentsDetailsInfoForActiveStudnetsVy
{
    public int StudentId { get; set; }

    public string Standard { get; set; } = null!;

    public string? Division { get; set; }

    public string? GardianName { get; set; }

    public string? ReferralCode { get; set; }

    public decimal RegistrationAmount { get; set; }

    public string? ProfileUrl { get; set; }

    public bool? IsActive { get; set; }

    public bool IsPaid { get; set; }

    public DateTime? TrialPeriodTill { get; set; }

    public string? RollNo { get; set; }

    public bool? SharingPermission { get; set; }

    public string? LevelPath { get; set; }

    public string? GuardianEmailId { get; set; }

    public string? GuardianMobileNo { get; set; }

    public int? InstituteId { get; set; }

    public string? InstituteName { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? CityName { get; set; }

    public int? Coins { get; set; }

    public long? Rank { get; set; }

    public string? Gender { get; set; }

    public DateTime? Dob { get; set; }

    public string? AddressLine1 { get; set; }

    public string? Pincode { get; set; }

    public string? MobileNo { get; set; }

    public string? PhotoUrl { get; set; }

    public bool? IsMobileVerified { get; set; }

    public bool? IsEmailVerified { get; set; }

    public bool? IsUserDeactivated { get; set; }

    public string? EmailId { get; set; }

    public DateOnly? RegistrationDate { get; set; }

    public string? RegistrationDisplayDate { get; set; }

    public string? Username { get; set; }

    public string? Password { get; set; }

    public int? Gems { get; set; }

    public bool? IsAllocated { get; set; }

    public string? GroupName { get; set; }

    public string? MainCourseName { get; set; }
}
