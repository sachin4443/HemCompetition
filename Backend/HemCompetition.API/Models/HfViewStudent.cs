using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfViewStudent
{
    public int StudentId { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? MobileNo { get; set; }

    public string? GardianName { get; set; }

    public string? EmailId { get; set; }

    public string Standard { get; set; } = null!;

    public string? Division { get; set; }

    public int? InstituteId { get; set; }
}
