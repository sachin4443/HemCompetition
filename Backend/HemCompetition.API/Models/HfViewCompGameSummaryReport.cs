using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfViewCompGameSummaryReport
{
    public int? UserId { get; set; }

    public string? UserName { get; set; }

    public string? Gender { get; set; }

    public string? MobileNo { get; set; }

    public string? SchoolName { get; set; }

    public string? EmailId { get; set; }

    public string? CityName { get; set; }

    public string Standard { get; set; } = null!;

    public string Division { get; set; } = null!;

    public string? Categories { get; set; }

    public int? Modules { get; set; }

    public int? ValuesCompleted { get; set; }

    public decimal Game24Score { get; set; }

    public int Game24TotalTime { get; set; }

    public decimal FutoshikiScore { get; set; }

    public int FutoshikiTotalTime { get; set; }

    public decimal KenKenScore { get; set; }

    public int KenKenTotalTime { get; set; }

    public int TotalMarksObtain { get; set; }

    public int TotalTime { get; set; }

    public decimal PuzzleScore { get; set; }

    public int PuzzleTotalTime { get; set; }

    public decimal ScrambleScore { get; set; }

    public int ScrambleTotalTime { get; set; }

    public decimal WordGameScore { get; set; }

    public int WordGameTotalTime { get; set; }

    public decimal? TotalScore { get; set; }

    public int? TTimeInSecond { get; set; }

    public int? TTimeInMinutes { get; set; }

    public decimal? Minutes { get; set; }
}
