using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class PuzzleCompMaster
{
    public string? Size { get; set; }

    public DateTime? EntryDate { get; set; }

    public int? CourseId { get; set; }

    public string? Difficulty { get; set; }

    public string? Answer { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsDeleted { get; set; }

    public int? IsCreatedBy { get; set; }

    public int? IsDeleteBy { get; set; }

    public int? Pzid { get; set; }
}
