using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfTblSchoolVisitActivity
{
    public int ActivityId { get; set; }

    public int? VisitId { get; set; }

    public string ActivityType { get; set; } = null!;

    public string? Remarks { get; set; }

    public string? FilePath { get; set; }

    public int? EntId { get; set; }

    public DateTime? EntDate { get; set; }
}
