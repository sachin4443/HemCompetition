using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class TotalStudent
{
    public int InstituteId { get; set; }

    public string? Name { get; set; }

    public int? TotalStud { get; set; }
}
