using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfProject
{
    public int ProjectId { get; set; }

    public string? ProjectName { get; set; }

    public int? CourseId { get; set; }

    public int? ProjectTypeId { get; set; }
}
