using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class Student
{
    public int StudentId { get; set; }

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

    public string? UserName { get; set; }

    public string? UserPassword { get; set; }

    public bool? IsActive { get; set; }

    public int CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string TallyName { get; set; } = null!;

    public string? OldTallyName { get; set; }

    public int? Guardian { get; set; }
}
