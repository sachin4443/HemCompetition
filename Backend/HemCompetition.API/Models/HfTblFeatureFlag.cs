using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfTblFeatureFlag
{
    public int FeatureId { get; set; }

    public string FeatureKey { get; set; } = null!;

    public string? FeatureValue { get; set; }

    public string? Description { get; set; }

    public int? RecStatus { get; set; }
}
