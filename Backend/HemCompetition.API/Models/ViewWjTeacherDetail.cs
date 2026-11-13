using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class ViewWjTeacherDetail
{
    public int TeacherId { get; set; }

    public string? Standard { get; set; }

    public string? ProfileUrl { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Gender { get; set; }

    public DateTime? Dob { get; set; }

    public string? MobileNo { get; set; }

    public string? EmailId { get; set; }

    public string? Pincode { get; set; }

    public string? CityName { get; set; }

    public string? PhotoUrl { get; set; }

    public int? InstituteId { get; set; }

    public string? InstituteName { get; set; }

    public bool? IsActive { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string? UserName { get; set; }

    public string? Password { get; set; }

    public int? Coins { get; set; }

    public long? Rank { get; set; }

    public long? SchoolRank { get; set; }

    public int Credits { get; set; }

    public int? TotalCertification { get; set; }

    public int? HemVirtueStudentCount { get; set; }

    public int? HemVirtueStudentStudentMcqcount { get; set; }

    public int? HemVirtueStudentStudentPracticalCount { get; set; }

    public int? HemVirtueStudentStudentPracticalUncheckedCount { get; set; }

    public int? StudentCertificationCount { get; set; }
}
