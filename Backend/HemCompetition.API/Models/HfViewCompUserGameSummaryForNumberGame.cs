using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfViewCompUserGameSummaryForNumberGame
{
    public int? UserId { get; set; }

    public string? UserName { get; set; }

    public string? UserRoles { get; set; }

    public string? MobileNo { get; set; }

    public string? EmailId { get; set; }

    public string? CityName { get; set; }

    public string? SchoolName { get; set; }

    public int? FutoshikiAnswer { get; set; }

    public int? FutoshikiSkipped { get; set; }

    public int? FutoshikiNext { get; set; }

    public decimal? FutoshikiScore { get; set; }

    public int? FutoshikiTotalTime { get; set; }

    public int? Game24Answer { get; set; }

    public int? Game24Skipped { get; set; }

    public int? Game24Next { get; set; }

    public decimal? Game24Score { get; set; }

    public int? Game24TotalTime { get; set; }

    public int? KenKenAnswer { get; set; }

    public int? KenKenSkipped { get; set; }

    public int? KenKenNext { get; set; }

    public decimal? KenKenScore { get; set; }

    public int? KenKenTotalTime { get; set; }

    public decimal? TotalScore { get; set; }

    public int? TotalTime { get; set; }
}
