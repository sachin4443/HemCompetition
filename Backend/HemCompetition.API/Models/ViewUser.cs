using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class ViewUser
{
    public int? UserId { get; set; }

    public string? UserCode { get; set; }

    public string? Title { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Gender { get; set; }

    public int? MaritalStatus { get; set; }

    public DateTime? Dob { get; set; }

    public string? MobileNo { get; set; }

    public string? PhoneNo { get; set; }

    public string? EmailId { get; set; }

    public string? AddressLine1 { get; set; }

    public string? AddressLine2 { get; set; }

    public string? AddressLine3 { get; set; }

    public int? City { get; set; }

    public string? Pincode { get; set; }

    public string? PhotoUrl { get; set; }

    public bool? IsActive { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string? AspNetUserId { get; set; }

    public string? RegisterFrom { get; set; }

    public string? AccessToken { get; set; }

    public string? CityName { get; set; }

    public string? Password { get; set; }

    public bool? IsMobileVerified { get; set; }

    public bool? IsEmailVerified { get; set; }

    public string? LanguagePreference { get; set; }

    public bool? IsUserDeactivated { get; set; }

    public string Name { get; set; } = null!;

    public string AspNetUsersId { get; set; } = null!;

    public string UserName { get; set; } = null!;

    public string? UserRoles { get; set; }

    public string? UserRolesId { get; set; }
}
