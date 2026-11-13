using Dapper;
using HemCompetition.API.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HemCompetition.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Game24ApiController : ControllerBase
    {
        private readonly string _connectionString;

        public Game24ApiController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("ConnectionString 'DefaultConnection' not found.");
        }

        [HttpGet("GetQuestions/{userId}")]
        public async Task<IActionResult> GetQuestions(long userId, CancellationToken cancellationToken)
        {
            try
            {
                await using var conn = new SqlConnection(_connectionString);

                var initParams = new DynamicParameters();
                initParams.Add("@Userid", userId);
                initParams.Add("@GameName", "Game24");
                initParams.Add("@GameID", 0);

                var initCmd = new CommandDefinition(
                    "dbo.Sp_PlayGame_Logic",
                    initParams,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken
                );

                await conn.ExecuteAsync(initCmd);

                // Step 2: Fetch actual Questions ('Lokup_G24')
                var fetchParams = new DynamicParameters();
                fetchParams.Add("@Userid", userId);
                fetchParams.Add("@GameName", "Lokup_G24");
                fetchParams.Add("@GameID", 0);

                var fetchCmd = new CommandDefinition(
                    "dbo.Sp_PlayGame_Logic",
                    fetchParams,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken
                );

                var questions = await conn.QueryAsync<Game24QuestionDto>(fetchCmd);

                return Ok(new
                {
                    success = true,
                    message = "Questions loaded successfully.",
                    data = questions
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error fetching questions from DB.",
                    error = ex.Message
                });
            }
        }

        [HttpPost("SaveAnswer")]
        public async Task<IActionResult> SaveAnswer([FromBody] Save24AnswerDto dto, CancellationToken cancellationToken)
        {
            if (dto == null || dto.GCID <= 0 || dto.UserId <= 0)
            {
                return BadRequest(new { success = false, message = "Invalid input data." });
            }

            try
            {
                await using var conn = new SqlConnection(_connectionString);

                DateTime start = dto.StartTime.HasValue
                    ? dto.StartTime.Value.ToLocalTime()
                    : DateTime.Now;

                DateTime end = dto.EndTime.HasValue
                    ? dto.EndTime.Value.ToLocalTime()
                    : DateTime.Now;

                string startTimeStr = start.ToString("yyyy-MM-dd HH:mm:ss");
                string endTimeStr = end.ToString("yyyy-MM-dd HH:mm:ss");

                TimeSpan duration = end - start;

                string timeOffStr = "0";
                if (duration.TotalSeconds > 0)
                {
                    int totalSeconds = (int)Math.Max(0, duration.TotalSeconds);
                     timeOffStr = totalSeconds.ToString();      
                }
                var saveParams = new DynamicParameters();
                saveParams.Add("@Userid", dto.UserId);
                saveParams.Add("@GameID", dto.GCID);
                saveParams.Add("@UserAnswer", string.IsNullOrWhiteSpace(dto.UserAnswer) ? "0" : dto.UserAnswer);
                saveParams.Add("@StartTime", startTimeStr);
                saveParams.Add("@EndTime", endTimeStr);
                saveParams.Add("@Timeoff", timeOffStr);
                saveParams.Add("@GameAction", "Game24");

                var cmdSave = new CommandDefinition(
                    "dbo.SP_InesetNextGames",
                    saveParams,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken
                );

                await conn.ExecuteAsync(cmdSave);

                return Ok(new
                {
                    status = "OK",
                    success = true,
                    message = "Record Saved Successfully!"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Something went wrong while saving...",
                    error = ex.Message
                });
            }
        }
        [HttpPost("SkipQuestion")]
        public async Task<IActionResult> SkipQuestion([FromBody] SkipDto dto, CancellationToken cancellationToken)
        {
            if (dto == null || dto.GCID <= 0 || dto.UserId <= 0)
            {
                return BadRequest(new { success = false, message = "Invalid request payload." });
            }

            try
            {
                await using var conn = new SqlConnection(_connectionString);

                string startTimeStr = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string endTimeStr = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                var skipParams = new DynamicParameters();
                skipParams.Add("@Userid", dto.UserId);
                skipParams.Add("@GameID", dto.GCID);
                skipParams.Add("@StartTime", startTimeStr);
                skipParams.Add("@EndTime", endTimeStr);
                skipParams.Add("@GameAction", "Game24");

                var cmdSkip = new CommandDefinition(
                    "dbo.SP_SkipGames",
                    skipParams,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken
                );

                await conn.ExecuteAsync(cmdSkip);

                return Ok(new
                {
                    status = "1",
                    success = true,
                    message = "Record skipped successfully!"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "2",
                    success = false,
                    message = "Something went wrong while skipping...",
                    error = ex.Message
                });
            }
        }
    }
}