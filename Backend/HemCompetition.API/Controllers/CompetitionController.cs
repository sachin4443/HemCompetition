using Dapper;
using HemCompetition.API.DTOs; // Aapke DTOs yahan use ho rahe hain
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Data;
using System.Security.Claims;

namespace HemCompetition.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompetitionController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly IMemoryCache _cache;

        private static readonly TimeSpan CacheDuration = TimeSpan.FromSeconds(15);

        public CompetitionController(IConfiguration configuration, IMemoryCache cache)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("ConnectionString 'DefaultConnection' not found.");
            _cache = cache;
        }
        private IDbConnection CreateConnection() => new SqlConnection(_connectionString);
        [HttpGet("status/{userId}")]
        public async Task<IActionResult> GetModuleStatus(long userId, CancellationToken cancellationToken)
        {
            string cacheKey = $"overall_status_{userId}";
            try
            {
                await using var conn = new SqlConnection(_connectionString);
                var dynamicParams = new DynamicParameters();
                dynamicParams.Add("@USER_ID", userId, DbType.Int64);

                var row = await conn.QueryFirstOrDefaultAsync(
                    new CommandDefinition(
                        "HF_SP_GetModules_Play_Status",
                        dynamicParams,
                        commandType: CommandType.StoredProcedure,
                        commandTimeout: 10,
                        cancellationToken: cancellationToken
                    )
                );

                var response = new ModuleStatusDto();
                if (row != null)
                {
                    response.Word_Game_Status = Convert.ToString(row.Word_Game_Status) ?? "S";
                    response.Quiz_Game_Status = Convert.ToString(row.Quiz_Game_Status) ?? "S";
                    response.Number_Game_Status = Convert.ToString(row.Number_Game_Status) ?? "S";
                }
                return Ok(response);
            }
            catch (Exception)
            {
                return Ok(new ModuleStatusDto()); // Fallback DTO (defaults to "S")
            }
        }

        [HttpGet("numbergame/status/{userId}")]
        public async Task<IActionResult> GetNumberGameStatus(long userId, CancellationToken cancellationToken)
        {
            string cacheKey = $"number_status_{userId}";
            try
            {
                await using var conn = new SqlConnection(_connectionString);
                var dynamicParams = new DynamicParameters();
                dynamicParams.Add("@USER_ID", userId, DbType.Int64);

                var row = await conn.QueryFirstOrDefaultAsync(
                    new CommandDefinition(
                        "HF_SP_GetNumberGame_Play_Status",
                        dynamicParams,
                        commandType: CommandType.StoredProcedure,
                        commandTimeout: 10,
                        cancellationToken: cancellationToken
                    )
                );

                var response = new NumberGameStatusDto();
                if (row != null)
                {
                    response.kenken_Game_Status = Convert.ToString(row.kenken_Game_Status) ?? "S";
                    response.Futoshiki_Game_Status = Convert.ToString(row.Futoshiki_Game_Status) ?? "S";
                    response.Game24_Game_Status = Convert.ToString(row.Game24_Game_Status) ?? "S";
                }
                return Ok(response);
            }
            catch (Exception)
            {
                return Ok(new NumberGameStatusDto());
            }
        }

        [HttpGet("wordgame/status/{userId}")]
        public async Task<IActionResult> GetWordGameStatus(long userId, CancellationToken cancellationToken)
        {
            string cacheKey = $"word_status_{userId}";
            try
            {
                await using var conn = new SqlConnection(_connectionString);
                var dynamicParams = new DynamicParameters();
                dynamicParams.Add("@USER_ID", userId, DbType.Int64);

                var row = await conn.QueryFirstOrDefaultAsync(
                    new CommandDefinition(
                        "HF_SP_GetWordGame_Play_Status",
                        dynamicParams,
                        commandType: CommandType.StoredProcedure,
                        commandTimeout: 10,
                        cancellationToken: cancellationToken
                    )
                );

                var response = new WordGameStatusDto();
                if (row != null)
                {
                    response.Scramble_Game_Status = Convert.ToString(row.Scramble_Game_Status) ?? "S";
                    response.Puzzle_Game_Status = Convert.ToString(row.Puzzle_Game_Status) ?? "S";
                    response.Word_Game_Status = Convert.ToString(row.Word_Game_Status) ?? "S";
                }
                return Ok(response);
            }
            catch (Exception)
            {
                return Ok(new WordGameStatusDto());
            }
        }

        [HttpGet("quiz/status/{userId}")]
        public async Task<IActionResult> GetQuizStatus(long userId, CancellationToken cancellationToken)
        {
            string cacheKey = $"quiz_status_{userId}";
            try
            {
                await using var conn = new SqlConnection(_connectionString);
                var dynamicParams = new DynamicParameters();
                dynamicParams.Add("@USER_ID", userId, DbType.Int64);

                var row = await conn.QueryFirstOrDefaultAsync(
                    new CommandDefinition(
                        "HF_SP_GetQuiz_Play_Status",
                        dynamicParams,
                        commandType: CommandType.StoredProcedure,
                        commandTimeout: 10,
                        cancellationToken: cancellationToken
                    )
                );

                var response = new QuizStatusDto();
                if (row != null)
                {
                    response.IsQuiz1Locked = Convert.ToInt32(row.IsQuiz1Locked ?? 0);
                    response.IsQuiz2Locked = Convert.ToInt32(row.IsQuiz2Locked ?? 0);
                    response.IsQuiz3Locked = Convert.ToInt32(row.IsQuiz3Locked ?? 0);
                    response.IsQuiz4Locked = Convert.ToInt32(row.IsQuiz4Locked ?? 0);
                }
                return Ok(response);
            }
            catch (Exception)
            {
                return Ok(new QuizStatusDto()); // Defaults to 0
            }
        }
        [HttpGet("get-questions")]
        public async Task<IActionResult> GetQuestions(int id, int levelId, int topicId, int langId, [FromQuery] string? isCertificate = null, CancellationToken cancellationToken = default)
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Invalid user session claims." });
            }
            try
            {
                await using var connection = new SqlConnection(_connectionString);
                var parameters = new
                {
                    QuizID = id,
                    TopicID = topicId,
                    UserID = userId,
                    LevelID = levelId,
                    IsCertificate = isCertificate,
                    LangId = langId
                };

                using (var multi = await connection.QueryMultipleAsync(
                    new CommandDefinition("HF_SP_CreateAndGetExamQuestions", parameters, commandType: CommandType.StoredProcedure, cancellationToken: cancellationToken)))
                {
                    int examId = (await multi.ReadAsync<int>()).FirstOrDefault();

                    var rawQuestions = (await multi.ReadAsync()).ToList();

                    var rawOptions = (await multi.ReadAsync()).ToList();

                    var questions = rawQuestions.Select(x => new QuestionViewModel1
                    {
                        QuestionID = (int?)((IDictionary<string, object>)x)["QuestionID"],
                        QuestionName = (string)((IDictionary<string, object>)x)["QuestionName"] ?? string.Empty,
                        Marks = (int?)((IDictionary<string, object>)x)["Marks"],
                        Type = (string)((IDictionary<string, object>)x)["Type"] ?? string.Empty,
                        ComplexityLevel = (string)((IDictionary<string, object>)x)["ComplexityLevel"] ?? string.Empty,
                        AnswerDescription = (string)((IDictionary<string, object>)x)["AnswerDescription"] ?? string.Empty,
                        CourseID = (int?)((IDictionary<string, object>)x)["CourseID"],
                        TopicID = (int?)((IDictionary<string, object>)x)["TopicID"]
                    }).ToList();

                    var options = rawOptions.Select(x => new QuestionOptionViewModel
                    {
                        OptionID = (int?)((IDictionary<string, object>)x)["OptionID"],
                        QuestionID = (int?)((IDictionary<string, object>)x)["QuestionID"],
                        Options = (string)((IDictionary<string, object>)x)["Options"] ?? string.Empty,
                        IsAnswer = Convert.ToBoolean(((IDictionary<string, object>)x)["IsAnswer"] ?? false)
                    }).ToList();

                    foreach (var q in questions)
                    {
                        q.QuestionOption = options
                            .Where(o => o.QuestionID == q.QuestionID && o.OptionID != null && !string.IsNullOrEmpty(o.Options) && o.Options.ToUpper() != "NULL")
                            .ToList();

                        foreach (var opt in q.QuestionOption)
                        {
                            opt.IsAnswer = false;
                        }
                    }

                    return Ok(new
                    {
                        Success = true,
                        Message = examId.ToString(),
                        Data = questions
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message, Data = (object?)null });
            }
        }

        [HttpGet("get-exam-result")]
        public async Task<IActionResult> GetExamResult(int examID, string isIQuiz, string iscertificate, string isAttempt)
        {
            await using var connection = new SqlConnection(_connectionString);
            var em = new ExamResultModel();

            em.CorrectQuestion = await connection.ExecuteScalarAsync<int>("SELECT COUNT(1) FROM ExaminationRecords WHERE ExaminationID = @ExamID AND isCorrect = 1", new { ExamID = examID });
            em.InCorrectQuestion = await connection.ExecuteScalarAsync<int>("SELECT COUNT(1) FROM ExaminationRecords WHERE ExaminationID = @ExamID AND isCorrect = 0", new { ExamID = examID });
            em.UnattemptedQuestion = await connection.ExecuteScalarAsync<int>("SELECT COUNT(1) FROM ExaminationRecords WHERE ExaminationID = @ExamID AND isCorrect IS NULL", new { ExamID = examID });
            em.ExamID = examID;

            var updateExam = "UPDATE Examination SET MarksObtain = @Marks, EndTime = GETDATE(), ModifiedDate = GETDATE() WHERE ExaminationID = @ExamID";
            await connection.ExecuteAsync(updateExam, new { Marks = em.CorrectQuestion, ExamID = examID });

            return Ok(new { Success = true, Message = "Exam result updated successfully", Data = em });
        }

        [HttpPost("submit-answer")]
        public async Task<IActionResult> SubmitAnswer([FromBody] ExamSubmitModel model)
        {
            await using var connection = new SqlConnection(_connectionString);

            var rightAnswerQuery = "SELECT IsAnswer FROM QuestionOption WHERE OptionID = @SelectedOption";
            bool isRight = await connection.ExecuteScalarAsync<bool>(rightAnswerQuery, new { SelectedOption = model.selectedOption });

            var updateQuery = @"UPDATE ExaminationRecords 
                                SET Answer = @SelectedOption, isCorrect = @IsCorrect, SubmitTiming = GETDATE(), ModifiedDate = GETDATE() 
                                WHERE ExaminationID = @ExamID AND QuestionID = @QuestionID";

            await connection.ExecuteAsync(updateQuery, new
            {
                SelectedOption = model.selectedOption,
                IsCorrect = isRight,
                ExamID = model.ExamID,
                QuestionID = model.questionID
            });

            return Ok(new { Success = true, Message = "Answer submitted successfully." });
        }

        [HttpGet("start-exam")]
        public async Task<IActionResult> StartExamQuiz(int quizId, int topicID, int levelID, string isInstituteQuiz, string iscertificate)
        {
            await using var connection = new SqlConnection(_connectionString);
            var model = new ExamModel
            {
                QuizTime = 10,
                QuizId = quizId,
                LevelID = levelID,
                IsInstituteQuiz = isInstituteQuiz,
                Iscertificate = iscertificate
            };

            if (string.IsNullOrEmpty(iscertificate))
            {
                var query = "SELECT Type, Duration, StartDateTime, EndDateTime FROM Quiz WHERE QuizID = @QuizId";
                var timer = await connection.QueryFirstOrDefaultAsync<QuizTimerDto>(query, new { QuizId = quizId });

                if (timer != null)
                {
                    if (timer.Type == "Flexible" && timer.Duration.HasValue)
                    {
                        model.QuizTime = timer.Duration.Value;
                    }
                    else if (timer.StartDateTime.HasValue && timer.EndDateTime.HasValue)
                    {
                        TimeSpan span = timer.EndDateTime.Value.Subtract(timer.StartDateTime.Value);
                        model.QuizTime = (int)span.TotalMinutes;
                    }
                }
            }
            else
            {
                model.QuizTime = 10;
            }

            return Ok(new { Data = model, Message = "Data Loaded Successfully", Success = true });
        }

        [HttpGet("review-quiz")]
        public async Task<IActionResult> ReviewQuiz([FromQuery] int id)
        {
            try
            {
                var examBAL = new ExamBAL(_connectionString);
                var reviewData = examBAL.GetUserExamDetials(id);

                return Ok(new
                {
                    Success = true,
                    Data = reviewData,
                    Message = "Success"
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    Success = false,
                    Data = (object?)null,
                    Message = ex.Message
                });
            }
        }

        #region Puzzle

        [HttpGet("puzzle-init")]
        public async Task<IActionResult> GetPuzzleInitialization(CancellationToken cancellationToken)
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;
            long userId = 0;
            if (!string.IsNullOrEmpty(userIdClaim))
            {
                long.TryParse(userIdClaim, out userId);
            }

            try
            {
                await using var conn = new SqlConnection(_connectionString);

                var parameters = new DynamicParameters();
                parameters.Add("@UserID", userId, DbType.Int64);
                parameters.Add("@puzzleSolvedCount", dbType: DbType.Int32, direction: ParameterDirection.Output);
                parameters.Add("@puzzleskippedCount", dbType: DbType.Int32, direction: ParameterDirection.Output);

                await conn.ExecuteAsync(
                    "puzzleSolvedCount",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                int puzzleSolvedCount = parameters.Get<int>("@puzzleSolvedCount");
                int puzzleSkippedCount = parameters.Get<int>("@puzzleskippedCount");

                if (puzzleSolvedCount >= 5 || puzzleSkippedCount > 0)
                {
                    return Ok(new { success = false, message = "You have already completed the puzzle games!" });
                }

                string courseQuery = $"SELECT Answer FROM dbo.Puzzle_Comp_Master WHERE Answer NOT IN (SELECT TOP {puzzleSolvedCount * 5} Answer FROM dbo.Puzzle_Comp_Master)";
                var words = await conn.QueryAsync<string>(courseQuery);

                return Ok(new { success = true, words = words.AsList(), solvedCount = puzzleSolvedCount });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost("puzzle-save")]
        public async Task<IActionResult> SavePuzzle([FromBody] PuzzleSaveDto model, CancellationToken cancellationToken)
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;
            long userId = 0;
            if (!string.IsNullOrEmpty(userIdClaim))
            {
                long.TryParse(userIdClaim, out userId);
            }

            try
            {
                await using var conn = new SqlConnection(_connectionString);

                var parameters = new DynamicParameters();
                parameters.Add("@IsResolved", "Y");
                parameters.Add("@IsResolvedBy", userId);
                parameters.Add("@StartTime", model.StartTime);
                parameters.Add("@EndTime", model.EndTime);
                parameters.Add("@TimeDiff", model.TimeDiff);
                parameters.Add("@UserAnswer", model.UserAnswer);
                parameters.Add("@Remark", null);

                int rowsAffected = await conn.ExecuteAsync(
                    "HF_SP_SavePuzzleTransaction",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                if (rowsAffected > 0)
                {
                    return Ok(new { success = true, message = "Record Save Successful" });
                }

                return BadRequest(new { success = false, message = "Record Save Unsuccessful" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost("skip-puzzle")]
        public async Task<IActionResult> SkipPuzzle([FromBody] PuzzleSaveDto model, CancellationToken cancellationToken)
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;
            long userId = 0;
            if (!string.IsNullOrEmpty(userIdClaim))
            {
                long.TryParse(userIdClaim, out userId);
            }

            try
            {
                await using var conn = new SqlConnection(_connectionString);

                var parameters = new DynamicParameters();
                parameters.Add("@IsResolved", "Y");
                parameters.Add("@IsResolvedBy", userId);
                parameters.Add("@StartTime", model.StartTime);
                parameters.Add("@EndTime", model.EndTime);
                parameters.Add("@TimeDiff", model.TimeDiff);
                parameters.Add("@UserAnswer", model.UserAnswer ?? string.Empty);
                parameters.Add("@Remark", "Skipped");

                int rowsAffected = await conn.ExecuteAsync(
                    "HF_SP_SavePuzzleTransaction",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                if (rowsAffected > 0)
                {
                    return Ok(new { success = true, message = "Puzzle Skipped Successfully" });
                }

                return BadRequest(new { success = false, message = "Failed to Skip Puzzle" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
        #endregion

        #region WordGame
        private int GetUserId()
        {
            var claim = User.FindFirst("UserId")
                     ?? User.FindFirst(ClaimTypes.NameIdentifier)
                     ?? User.FindFirst("studentId")
                     ?? User.FindFirst("id");

            return claim != null && int.TryParse(claim.Value, out int id) ? id : 0;
        }

        [HttpGet("wordgame-init")]
        public async Task<IActionResult> InitWordGame()
        {
            int userId = GetUserId();
            if (userId == 0) return Unauthorized(new { success = false, message = "User not authenticated" });

            using var db = CreateConnection();

            // Step 1: Check Solved and Skipped Counts via SP
            var p = new DynamicParameters();
            p.Add("@UserID", userId, DbType.Int32, ParameterDirection.Input);
            p.Add("@WordGameSolvedCount", dbType: DbType.Int32, direction: ParameterDirection.Output);
            p.Add("@WordGameskippedCount", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await db.ExecuteAsync("WordGameSolvedCount", p, commandType: CommandType.StoredProcedure);

            int solvedCount = p.Get<int>("@WordGameSolvedCount");
            //int skippedCount = p.Get<int>("@WordGameskippedCount");
            int skippedCount = 0;

            if ((solvedCount + skippedCount) >= 5)
            {
                return Ok(new WordGameInitResponseDto
                {
                    Success = false,
                    Message = "You have already completed the word games!",
                    Words = Enumerable.Empty<WordGameDataDto>()
                });
            }

            // Step 2: Retrieve word game records via SP
            var words = await db.QueryAsync<WordGameDataDto>(
                "HF_WordGamedata_Comp",
                new { UserID = userId },
                commandType: CommandType.StoredProcedure
            );

            return Ok(new WordGameInitResponseDto
            {
                Success = true,
                Message = "Data loaded successfully.",
                Words = words
            });
        }

        /// <summary>
        /// Save Word Game answer or skip action
        /// </summary>
        [HttpPost("wordgame-save")]
        public async Task<IActionResult> SaveWordGame([FromBody] WordGameSaveRequestDto model)
        {
            int userId = GetUserId();
            if (userId == 0) return Unauthorized(new[] { "2", "User not authenticated" });

            using var db = CreateConnection();

            var saveParams = new DynamicParameters();
            saveParams.Add("@WGID", model.Game24ID);
            saveParams.Add("@WGTopicID", model.GameTopicID);
            saveParams.Add("@UserAnswer", model.UserAnswer ?? "0");
            saveParams.Add("@IsCreatedBy", userId);
            saveParams.Add("@StartTime", model.StartTime);
            saveParams.Add("@EndTime", model.EndTime);
            saveParams.Add("@TimeDiff", model.TimeDiff);
            saveParams.Add("@Remark", model.Remark);

            int rows = await db.ExecuteAsync("HF_SP_SaveWordGameSolution", saveParams, commandType: CommandType.StoredProcedure);

            if (rows > 0)
            {
                return Ok(new List<string> { "1", "Record Save Successful" });
            }

            return Ok(new List<string> { "0", "Record not Saved." });
        }
        #endregion

        #region Scrumble
        [HttpGet("scramble_init")]
        public async Task<IActionResult> InitScramble()
        {
            int userId = GetUserId();
            if (userId == 0) return Unauthorized(new { success = false, message = "User not authenticated" });

            using var db = CreateConnection();

            // Step 1: Check Scramble Solved and Skipped counts via SP
            var p = new DynamicParameters();
            p.Add("@UserID", userId, DbType.Int32, ParameterDirection.Input);
            p.Add("@ScrambleSolvedCount", dbType: DbType.Int32, direction: ParameterDirection.Output);
            p.Add("@ScrambleSkippedCount", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await db.ExecuteAsync("ScrambleSolvedCount", p, commandType: CommandType.StoredProcedure);

            int solvedCount = p.Get<int>("@ScrambleSolvedCount");
            int skippedCount = p.Get<int>("@ScrambleSkippedCount");

            if ((solvedCount+ skippedCount) >= 5 )
            {
                return Ok(new
                {
                    success = false,
                    message = "You have completed all the Games!",
                    results = Enumerable.Empty<object>()
                });
            }

            // Step 2: Fetch scramble word rows via SP
            var results = await db.QueryAsync<dynamic>(
                "SP_ScrambleWord_Comp",
                new { UserID = userId },
                commandType: CommandType.StoredProcedure
            );

            var list = results.ToList();
            if (!list.Any())
            {
                return Ok(new
                {
                    success = false,
                    message = "You have completed all the Games!",
                    results = Enumerable.Empty<object>()
                });
            }

            var first = list.First();

            return Ok(new
            {
                success = true,
                question = first.Question,
                questionId = first.QuestionId,
                topicId = first.TopicId,
                answer = first.Answer,
                results = list
            });
        }

        [HttpPost("scramble_save")]
        public async Task<IActionResult> SaveScramble([FromBody] ScrambleSaveRequestDto model)
        {
            int userId = GetUserId();
            if (userId == 0) return Unauthorized(new[] { "2", "User not authenticated" });

            using var db = CreateConnection();

            // Step 1: Verify counts via SP
            var p = new DynamicParameters();
            p.Add("@UserID", userId, DbType.Int32, ParameterDirection.Input);
            p.Add("@ScrambleSolvedCount", dbType: DbType.Int32, direction: ParameterDirection.Output);
            p.Add("@ScrambleSkippedCount", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await db.ExecuteAsync("ScrambleSolvedCount", p, commandType: CommandType.StoredProcedure);

            int solvedCount = p.Get<int>("@ScrambleSolvedCount");
            int skippedCount = p.Get<int>("@ScrambleSkippedCount");

            if ((solvedCount+ skippedCount) >= 5)
            {
                return Ok(new List<string> { "2", "You have completed all the Games!" });
            }

            // Step 2: Execute Stored Procedure for saving the answer/skip
            var saveParams = new DynamicParameters();
            saveParams.Add("@Answer", model.Answer ?? "0");
            saveParams.Add("@UserId", userId);
            saveParams.Add("@StartTime", model.StartTime);
            saveParams.Add("@EndTime", model.EndTime);
            saveParams.Add("@TimeDiff", model.TimeDiff);
            saveParams.Add("@QuestionId", model.QuestionId);
            saveParams.Add("@ScrambleTopicId", model.TopicId);
            saveParams.Add("@Remark", model.Remark); // null for correct answers, "Skipped" for skips

            int rows = await db.ExecuteAsync("HF_SP_SaveScrambleSolution", saveParams, commandType: CommandType.StoredProcedure);

            if (rows > 0)
            {
                return Ok(new List<string> { "1", "Record Save Successful" });
            }

            return Ok(new List<string> { "0", "Record not Saved." });
        }
        #endregion

        [HttpGet("test-exception")]
        public IActionResult TriggerTestException()
        {
            int zero = 0;
            int result = 10 / zero;

            return Ok(result);
        }
    }
        
    }
