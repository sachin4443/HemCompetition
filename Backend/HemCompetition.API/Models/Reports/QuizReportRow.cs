public class QuizReportRow
{
    public int UserId { get; set; }
    public string? UserName { get; set; }
    public string? UserRoles { get; set; }
    public string? MobileNo { get; set; }
    public string? EmailID { get; set; }
    public string? CityName { get; set; }
    public string? SchoolName { get; set; }

    public decimal? Quiz_1 { get; set; }
    public int? Quiz1_Time_Score { get; set; }

    public decimal? Quiz_2 { get; set; }
    public int? Quiz2_Time_Score { get; set; }

    public decimal? Quiz_3 { get; set; }
    public int? Quiz3_Time_Score { get; set; }

    public decimal? Quiz_4 { get; set; }
    public int? Quiz4_Time_Score { get; set; }

    public decimal? TotalMarksObtain { get; set; }
    public int? TotalTime { get; set; }
}