using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class TblSchoolAccount
{
    public int AccountId { get; set; }

    public int ContactPersonId { get; set; }

    public string SchoolName { get; set; } = null!;

    public string? AddressLine1 { get; set; }

    public string? AddressLine2 { get; set; }

    public string? Area { get; set; }

    public int Country { get; set; }

    public int State { get; set; }

    public int City { get; set; }

    public string? Pincode { get; set; }

    public int? TotalStudents { get; set; }

    public int? Status { get; set; }

    public int ChannelPartner { get; set; }

    public int RegisteredBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? RemainderStatus { get; set; }

    public string? Remarks { get; set; }

    public string? SchoolBoard { get; set; }

    public string? SchoolFees { get; set; }
}
