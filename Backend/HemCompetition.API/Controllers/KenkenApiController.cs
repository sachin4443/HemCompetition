using System.Threading;
using System.Threading.Tasks;
using HemCompetition.API.DTOs;
using HemCompetition.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace HemCompetition.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KenkenApiController : ControllerBase
    {
        private readonly IKenkenService _kenkenService;

        public KenkenApiController(IKenkenService kenkenService)
        {
            _kenkenService = kenkenService;
        }

        [HttpGet("GetQuestions/{userId}")]
        public async Task<IActionResult> GetQuestions(long userId, CancellationToken cancellationToken)
        {
            var data = await _kenkenService.GetActiveQuestionsAsync(userId, cancellationToken);
            return Ok(new { success = true, data });
        }

        [HttpPost("SaveAnswer")]
        public async Task<IActionResult> SaveAnswer([FromBody] SaveKenkenAnswerDto dto, CancellationToken cancellationToken)
        {
            var result = await _kenkenService.ValidateAndSaveAnswerAsync(dto, cancellationToken);
            return Ok(result);
        }

        [HttpPost("SkipQuestion")]
        public async Task<IActionResult> SkipQuestion([FromBody] SkipKenkenDto dto, CancellationToken cancellationToken)
        {

            DateTime start = DateTime.Now;
            DateTime end = DateTime.Now;

            if (!string.IsNullOrEmpty(dto.StartTime) && DateTime.TryParse(dto.StartTime, out DateTime parsedStart))
            {
                start = parsedStart.ToLocalTime();
            }

            if (!string.IsNullOrEmpty(dto.EndTime) && DateTime.TryParse(dto.EndTime, out DateTime parsedEnd))
            {
                end = parsedEnd.ToLocalTime();
            }
            int totalSeconds = dto.TimeDiff > 0 ? dto.TimeDiff : (int)Math.Max(0, (end - start).TotalSeconds);

            string timeOffStr = totalSeconds.ToString();
            dto.TimeDiff = totalSeconds;

            var result = await _kenkenService.SkipQuestionAsync(dto, cancellationToken);
            return Ok(result);
        }
    }
}