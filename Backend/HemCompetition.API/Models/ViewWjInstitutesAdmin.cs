using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class ViewWjInstitutesAdmin
{
    public int InstituteId { get; set; }

    public string InstituteType { get; set; } = null!;

    public string? InstituteName { get; set; }

    public string? BannerImageUrl { get; set; }

    public string? ProfileUrl { get; set; }

    public bool? IsActive { get; set; }

    public bool Isverify { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? EmailId { get; set; }

    public string? MobileNo { get; set; }

    public string? AddressLine1 { get; set; }

    public string? CityName { get; set; }

    public string? Pincode { get; set; }

    public string? PhotoUrl { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string Username { get; set; } = null!;

    public string? Password { get; set; }

    public int? Coins { get; set; }

    public int? Ispaid { get; set; }

    public int? TotalStudentCount { get; set; }

    public int? Register { get; set; }

    public int? ActiveStudents { get; set; }

    public int? Rank { get; set; }

    public string? ReferralCode { get; set; }
}
