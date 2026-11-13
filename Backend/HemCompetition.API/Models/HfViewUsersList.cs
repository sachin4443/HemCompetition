using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfViewUsersList
{
    public int UserId { get; set; }

    public string? FirstName { get; set; }

    public string? MiddleName { get; set; }

    public string? LastName { get; set; }

    public string? Gender { get; set; }

    public DateTime? Dob { get; set; }

    public string? AddressLine1 { get; set; }

    public string? AddressLine2 { get; set; }

    public string? AddressLine3 { get; set; }

    public string? City { get; set; }

    public string? Pincode { get; set; }

    public string? State { get; set; }

    public string? PhotoUrl { get; set; }

    public int EntId { get; set; }

    public DateTime EntDate { get; set; }

    public int? UpdId { get; set; }

    public DateTime? UpdDate { get; set; }

    public string? LanguagePreference { get; set; }

    public int? RoleId { get; set; }

    public string? EmailId { get; set; }

    public string? PhoneNumber { get; set; }

    public string? UserName { get; set; }
}
