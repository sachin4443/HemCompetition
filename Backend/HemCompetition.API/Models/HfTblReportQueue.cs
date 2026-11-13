using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfTblReportQueue
{
    public int Id { get; set; }

    public int InstituteId { get; set; }

    public string UserEmail { get; set; } = null!;

    public DateTime? RequestDate { get; set; }

    public bool? IsProcessed { get; set; }

    public DateTime? ProcessedDate { get; set; }

    public string? FileUrl { get; set; }
}
