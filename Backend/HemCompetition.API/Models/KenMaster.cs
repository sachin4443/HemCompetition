using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class KenMaster
{
    public int Kmid { get; set; }

    public int? CourseId { get; set; }

    public string? Size { get; set; }

    public string? Operation { get; set; }

    public string? Difficulty { get; set; }

    public bool? IsActive { get; set; }

    public int? IsCreatedBy { get; set; }
}
