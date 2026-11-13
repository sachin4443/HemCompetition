using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class TotalActiveStudent
{
    public int? Students { get; set; }

    public int InstituteId { get; set; }

    public int? Coins { get; set; }
}
