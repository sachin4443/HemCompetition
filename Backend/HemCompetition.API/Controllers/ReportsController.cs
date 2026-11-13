using Dapper;
using HemCompetition.API.DTOs.Reports;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Text;

namespace HemCompetition.API.Controllers
{
    [Route("api/reports")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly string _connectionString;

        public ReportsController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        [HttpGet("live-summary")]
        public async Task<IActionResult> GetLiveSummary()
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);

                var quizRows = (
                    await conn.QueryAsync<QuizReportRow>(
                        "HF_SP_COMP_QuizReports",
                        commandType: CommandType.StoredProcedure
                    )
                ).ToList();


                var numberRows = (
                    await conn.QueryAsync<NumberGameReportRow>(
                        "HF_SP_Comp_UserGameSummaryForNumberGame",
                        commandType: CommandType.StoredProcedure
                    )
                ).ToList();


                var wordRows = (
                    await conn.QueryAsync<WordGameReportRow>(
                        "HF_SP_Comp_UserGameSummaryForWordGame",
                        commandType: CommandType.StoredProcedure
                    )
                ).ToList();
                var userIds = quizRows
                    .Select(x => x.UserId)
                    .Concat(numberRows.Select(x => x.UserId))
                    .Concat(wordRows.Select(x => x.UserId))
                    .Distinct()
                    .ToList();


                var result = new List<LiveParticipantDto>();


                foreach (var userId in userIds)
                {
                    var quizUserRows = quizRows
                        .Where(x => x.UserId == userId)
                        .ToList();

                    var number = numberRows
                        .FirstOrDefault(x => x.UserId == userId);

                    var word = wordRows
                        .FirstOrDefault(x => x.UserId == userId);

                    var baseRow =
                        quizUserRows.FirstOrDefault()
                        ?? number as dynamic
                        ?? word as dynamic;


                    if (baseRow == null)
                        continue;


                    var participant = new LiveParticipantDto
                    {
                        UserId = userId,

                        UserName =
                            quizUserRows.FirstOrDefault()?.UserName
                            ?? number?.UserName
                            ?? word?.UserName,

                        UserRoles =
                            quizUserRows.FirstOrDefault()?.UserRoles
                            ?? number?.UserRoles
                            ?? word?.UserRoles,

                        MobileNo =
                            quizUserRows.FirstOrDefault()?.MobileNo
                            ?? number?.MobileNo
                            ?? word?.MobileNo,

                        EmailID =
                            quizUserRows.FirstOrDefault()?.EmailID
                            ?? number?.EmailID
                            ?? word?.EmailID,

                        CityName =
                            quizUserRows.FirstOrDefault()?.CityName
                            ?? number?.CityName
                            ?? word?.CityName,

                        SchoolName =
                            quizUserRows.FirstOrDefault()?.SchoolName
                            ?? number?.SchoolName
                            ?? word?.SchoolName,

                        RoleCategory = GetRoleCategory(
                            quizUserRows.FirstOrDefault()?.UserRoles
                            ?? number?.UserRoles
                            ?? word?.UserRoles
                        )
                    };

                    participant.Quiz.Attempts = quizUserRows.Count;

                    foreach (var q in quizUserRows)
                    {
                        participant.Quiz.AttemptsList.Add(
                            new QuizAttemptDto
                            {
                                Quiz1 = q.Quiz_1 ?? 0,
                                Quiz1Time = q.Quiz1_Time_Score ?? 0,

                                Quiz2 = q.Quiz_2 ?? 0,
                                Quiz2Time = q.Quiz2_Time_Score ?? 0,

                                Quiz3 = q.Quiz_3 ?? 0,
                                Quiz3Time = q.Quiz3_Time_Score ?? 0,

                                Quiz4 = q.Quiz_4 ?? 0,
                                Quiz4Time = q.Quiz4_Time_Score ?? 0,

                                TotalMarksObtain =
                                    q.TotalMarksObtain ?? 0,

                                TotalTime =
                                    q.TotalTime ?? 0
                            }
                        );
                    }
                    var bestQuiz = quizUserRows
                        .OrderByDescending(x => x.TotalMarksObtain ?? 0)
                        .ThenBy(x => x.TotalTime ?? int.MaxValue)
                        .FirstOrDefault();

                    if (bestQuiz != null)
                    {
                        participant.Quiz.TotalMarksObtain =
                            bestQuiz.TotalMarksObtain ?? 0;

                        participant.Quiz.TotalTime =
                            bestQuiz.TotalTime ?? 0;

                        participant.Quiz.Quiz1 =
                            bestQuiz.Quiz_1 ?? 0;

                        participant.Quiz.Quiz2 =
                            bestQuiz.Quiz_2 ?? 0;

                        participant.Quiz.Quiz3 =
                            bestQuiz.Quiz_3 ?? 0;

                        participant.Quiz.Quiz4 =
                            bestQuiz.Quiz_4 ?? 0;
                    }

                    if (number != null)
                    {
                        participant.NumberGame =
                            new NumberGameSummaryDto
                            {
                                FutoshikiAnswer = number.FutoshikiAnswer,
                                FutoshikiSkipped = number.FutoshikiSkipped,
                                FutoshikiNext = number.FutoshikiNext,
                                FutoshikiScore = number.FutoshikiScore,
                                FutoshikiTotalTime = number.FutoshikiTotalTime,

                                Game24Answer = number.Game24Answer,
                                Game24Skipped = number.Game24Skipped,
                                Game24Next = number.Game24Next,
                                Game24Score = number.Game24Score,
                                Game24TotalTime = number.Game24TotalTime,

                                KenKenAnswer = number.KenKenAnswer,
                                KenKenSkipped = number.KenKenSkipped,
                                KenKenNext = number.KenKenNext,
                                KenKenScore = number.KenKenScore,
                                KenKenTotalTime = number.KenKenTotalTime,

                                TotalScore = number.TotalScore,
                                TotalTime = number.TotalTime
                            };
                    }

                    if (word != null)
                    {
                        participant.WordGame =
                            new WordGameSummaryDto
                            {
                                PuzzleAnswer = word.PuzzleAnswer,
                                PuzzleSkipped = word.PuzzleSkipped,
                                PuzzleNext = word.PuzzleNext,
                                PuzzleScore = word.PuzzleScore,
                                PuzzleTotalTime = word.PuzzleTotalTime,

                                ScrambleAnswer = word.ScrambleAnswer,
                                ScrambleSkipped = word.ScrambleSkipped,
                                ScrambleNext = word.ScrambleNext,
                                ScrambleScore = word.ScrambleScore,
                                ScrambleTotalTime = word.ScrambleTotalTime,

                                WordGameAnswer = word.WordGameAnswer,
                                WordGameSkipped = word.WordGameSkipped,
                                WordGameNext = word.WordGameNext,
                                WordGameScore = word.WordGameScore,
                                WordGameTotalTime = word.WordGameTotalTime,

                                TotalScore = word.TotalScore,
                                TotalTime = word.TotalTime
                            };
                    }


                    participant.HasQuizActivity =
                        quizUserRows.Any(x =>
                            (x.TotalMarksObtain ?? 0) > 0 ||
                            (x.TotalTime ?? 0) > 0
                        );

                    participant.HasNumberGameActivity =
                        number != null &&
                        number.TotalScore > 0;

                    participant.HasWordGameActivity =
                        word != null &&
                        word.TotalScore > 0;


                    participant.GamesStarted =
                        (participant.HasQuizActivity ? 1 : 0) +
                        (participant.HasNumberGameActivity ? 1 : 0) +
                        (participant.HasWordGameActivity ? 1 : 0);

                    participant.CombinedScore =
                        participant.Quiz.TotalMarksObtain
                        + participant.NumberGame.TotalScore
                        + participant.WordGame.TotalScore;


                    result.Add(participant);
                }


                return Ok(new
                {
                    success = true,

                    totalParticipants = result.Count,

                    students = result.Count(x =>
                        x.RoleCategory == "Student"
                    ),

                    teachers = result.Count(x =>
                        x.RoleCategory == "Teacher"
                    ),

                    participants = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    new
                    {
                        success = false,
                        message = ex.Message
                    }
                );
            }
        }

        private static string GetRoleCategory(string? role)
        {
            if (string.IsNullOrWhiteSpace(role))
                return "Other";

            var value = role.ToLowerInvariant();

            if (
                value.Contains("student") ||
                value.Contains("students")
            )
            {
                return "Student";
            }

            if (
                value.Contains("teacher") ||
                value.Contains("teachers")
            )
            {
                return "Teacher";
            }

            return "Other";
        }
        private async Task<IActionResult> ExportReportAsCsvAsync(string storedProcedure, string fileName)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                var rows = (await conn.QueryAsync(storedProcedure, commandType: CommandType.StoredProcedure)).Cast<IDictionary<string, object>>().ToList();

                if (!rows.Any())
                {
                    return NotFound(new { success = false, message = "No data available to export." });
                }

                var sb = new StringBuilder();

                var headers = rows.First().Keys.ToList();
                sb.AppendLine(string.Join(",", headers.Select(h => $"\"{h}\"")));

                foreach (var row in rows)
                {
                    var values = headers.Select(h =>
                    {
                        var val = row[h]?.ToString() ?? "";
                        return $"\"{val.Replace("\"", "\"\"")}\"";
                    });
                    sb.AppendLine(string.Join(",", values));
                }

                byte[] buffer = Encoding.UTF8.GetBytes(sb.ToString());
                return File(buffer, "text/csv", $"{fileName}_{DateTime.Now:yyyyMMdd}.csv");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Error generating report: {ex.Message}" });
            }
        }

        [HttpGet("quiz/download")]
        public async Task<IActionResult> DownloadQuizReport()
            => await ExportReportAsCsvAsync("HF_SP_COMP_QuizReports", "Quiz_Report");

        [HttpGet("number-game/download")]
        public async Task<IActionResult> DownloadNumberGameReport()
            => await ExportReportAsCsvAsync("HF_SP_Comp_UserGameSummaryForNumberGame", "NumberGame_Report");

        [HttpGet("word-game/download")]
        public async Task<IActionResult> DownloadWordGameReport()
            => await ExportReportAsCsvAsync("HF_SP_Comp_UserGameSummaryForWordGame", "WordGame_Report");

        [HttpGet("gamesummary/download")]
        public async Task<IActionResult> DownloadGameSummaryReport()
            => await ExportReportAsCsvAsync("HF_SP_COMP_GameSummaryReports", "GameSummary_Report");
        [HttpGet("detailsReport/download")]
        public async Task<IActionResult> DownloadDetailsReport()
          => await ExportReportAsCsvAsync("HF_SP_Comp_DetailReport", "Details_Report");


        [HttpGet("countReport/download")]
        public async Task<IActionResult> DownloadCountReport()
            => await ExportReportAsCsvAsync("HF_SP_Comp_CountReport", "Count_Report");
    }

}