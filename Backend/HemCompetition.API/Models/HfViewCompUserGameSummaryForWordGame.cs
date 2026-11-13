using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfViewCompUserGameSummaryForWordGame
{
    public int? UserId { get; set; }

    public string? UserName { get; set; }

    public string? UserRoles { get; set; }

    public string? MobileNo { get; set; }

    public string? EmailId { get; set; }

    public string? CityName { get; set; }

    public string? SchoolName { get; set; }

    public int? PuzzleAnswer { get; set; }

    public int? PuzzleSkipped { get; set; }

    public int? PuzzleNext { get; set; }

    public decimal? PuzzleScore { get; set; }

    public int? PuzzleTotalTime { get; set; }

    public int? ScrambleAnswer { get; set; }

    public int? ScrambleSkipped { get; set; }

    public int? ScrambleNext { get; set; }

    public decimal? ScrambleScore { get; set; }

    public int? ScrambleTotalTime { get; set; }

    public int? WordGameAnswer { get; set; }

    public int? WordGameSkipped { get; set; }

    public int? WordGameNext { get; set; }

    public decimal? WordGameScore { get; set; }

    public int? WordGameTotalTime { get; set; }

    public decimal? TotalScore { get; set; }

    public int? TotalTime { get; set; }
}
