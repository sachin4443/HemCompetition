using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class WjWordGameQuestion
{
    public int? WgtopicId { get; set; }

    public int? CourseId { get; set; }

    public string? WgQuestion { get; set; }

    public string? WgAnswer { get; set; }

    public bool? GameIsActive { get; set; }

    public int? Wgid { get; set; }
}
