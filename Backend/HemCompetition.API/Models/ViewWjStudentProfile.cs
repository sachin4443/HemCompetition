using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class ViewWjStudentProfile
{
    public int StudentId { get; set; }

    public string Standard { get; set; } = null!;

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? CityName { get; set; }

    public string? PhotoUrl { get; set; }

    public int? InstituteId { get; set; }

    public string? InstituteName { get; set; }

    public int? Coins { get; set; }

    public long? Rank { get; set; }
}
