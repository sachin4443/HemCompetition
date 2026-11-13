using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class VWorldEnviorment
{
    public int UserId { get; set; }

    public string? UserName { get; set; }

    public string? Name { get; set; }

    public TimeOnly? TimeScore { get; set; }

    public int? MarksObtain { get; set; }

    public DateTime? StartTime { get; set; }

    public DateTime? EndTime { get; set; }
}
