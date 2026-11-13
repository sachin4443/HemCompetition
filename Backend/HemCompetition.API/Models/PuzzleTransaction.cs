using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class PuzzleTransaction
{
    public int PztranId { get; set; }

    public int? CourseId { get; set; }

    public string? IsResolved { get; set; }

    public int? IsResolvedBy { get; set; }

    public DateTime? IsResolvedDate { get; set; }
}
