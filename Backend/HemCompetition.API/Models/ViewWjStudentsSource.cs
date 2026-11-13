using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class ViewWjStudentsSource
{
    public string? Source { get; set; }

    public int? SourceId { get; set; }

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
}
