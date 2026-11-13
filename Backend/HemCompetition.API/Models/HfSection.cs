using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfSection
{
    public int SectionId { get; set; }

    public int? ProjectId { get; set; }

    public string? SectionTitle { get; set; }

    public int? SortOrder { get; set; }
}
