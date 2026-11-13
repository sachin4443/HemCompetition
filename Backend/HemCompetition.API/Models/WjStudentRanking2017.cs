using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class WjStudentRanking2017
{
    public long RowNo { get; set; }

    public int? StudentId { get; set; }

    public string? Standard { get; set; }

    public string? StudentLevel { get; set; }

    public int? Coins { get; set; }

    public long? Rank { get; set; }

    public long? LevelRank { get; set; }

    public long? SchoolRank { get; set; }

    public int? Cups { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? EmailId { get; set; }

    public string? MobileNo { get; set; }

    public string? CityName { get; set; }

    public string? Division { get; set; }

    public string? GardianName { get; set; }

    public string? AddressLine1 { get; set; }

    public string? AddressLine3 { get; set; }

    public string? Pincode { get; set; }

    public int? InstituteId { get; set; }

    public string InstituteName { get; set; } = null!;

    public string? PhotoUrl { get; set; }

    public bool? IsPaid { get; set; }
}
