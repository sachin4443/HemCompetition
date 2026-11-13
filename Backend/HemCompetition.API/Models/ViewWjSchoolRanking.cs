using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class ViewWjSchoolRanking
{
    public int InstituteId { get; set; }

    public string? InstituteName { get; set; }

    public string? CityName { get; set; }

    public string? Pincode { get; set; }

    public int? NumberOfStudents { get; set; }

    public int Coins { get; set; }

    public long? Rank { get; set; }

    public long? CityRank { get; set; }
}
