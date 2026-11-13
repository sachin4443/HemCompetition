using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfTblStudent
{
    public int? StudentId { get; set; }

    public string Standard { get; set; } = null!;

    public string? Division { get; set; }

    public string? GardianName { get; set; }

    public string? RollNo { get; set; }

    public string? GuardianEmailId { get; set; }

    public string? GuardianMobileNo { get; set; }

    public int? InstituteId { get; set; }
}
