using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfViewCompWordAndNumberGameDetailReport
{
    public int? UserId { get; set; }

    public string? UserName { get; set; }

    public string? UserRoles { get; set; }

    public string? MobileNo { get; set; }

    public string? EmailId { get; set; }

    public string? CityName { get; set; }

    public string? SchoolName { get; set; }

    public string Answer { get; set; } = null!;

    public string? TimeDiff { get; set; }

    public string? StartTime { get; set; }

    public string? EndTime { get; set; }

    public string GameName { get; set; } = null!;

    public decimal? Score { get; set; }
}
