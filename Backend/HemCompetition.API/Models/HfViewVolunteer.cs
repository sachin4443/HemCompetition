using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfViewVolunteer
{
    public int UserId { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Gender { get; set; }

    public DateTime? Dob { get; set; }

    public string? MobileNo { get; set; }

    public string? EmailId { get; set; }

    public string? AddressLine1 { get; set; }

    public string? AddressLine2 { get; set; }

    public string? AddressLine3 { get; set; }

    public string? CityName { get; set; }

    public string? Pincode { get; set; }

    public bool? IsActive { get; set; }

    public int CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string AspNetUserId { get; set; } = null!;

    public string? UserName { get; set; }

    public string? Password { get; set; }

    public string? Role { get; set; }

    public string? ReferralCode { get; set; }

    public string? ReferralByCode { get; set; }

    public int? RefUserId { get; set; }

    public string? RefInstituteName { get; set; }
}
