using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfViewCompQuizReport
{
    public int? UserId { get; set; }

    public string? UserName { get; set; }

    public string? UserRoles { get; set; }

    public string? MobileNo { get; set; }

    public string? EmailId { get; set; }

    public string? CityName { get; set; }

    public string? SchoolName { get; set; }

    public int? Quiz1 { get; set; }

    public int? Quiz1TimeScore { get; set; }

    public int? Quiz2 { get; set; }

    public int? Quiz2TimeScore { get; set; }

    public int? Quiz3 { get; set; }

    public int? Quiz3TimeScore { get; set; }

    public int? Quiz4 { get; set; }

    public int? Quiz4TimeScore { get; set; }

    public int? TotalMarksObtain { get; set; }

    public int? TotalTime { get; set; }
}
