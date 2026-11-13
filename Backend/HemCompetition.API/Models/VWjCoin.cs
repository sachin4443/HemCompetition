using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class VWjCoin
{
    public int StudentId { get; set; }

    public string? StudentName { get; set; }

    public int InstituteId { get; set; }

    public string? SchoolName { get; set; }

    public string Standard { get; set; } = null!;

    public string Division { get; set; } = null!;

    public int? Coins { get; set; }

    public string? EmailId { get; set; }
}
