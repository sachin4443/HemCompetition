using System;
using System.Threading;
using System.Threading.Tasks;
using HemCompetition.API.DTOs;
using HemCompetition.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace HemCompetition.API.Controllers
{
    [Route("api/FutoshikiApi")]
    [ApiController]
    public class FutoshikiApiController : ControllerBase
    {
        private readonly IFutoshikiService _futoshikiService;

        public FutoshikiApiController(IFutoshikiService futoshikiService)
        {
            _futoshikiService = futoshikiService;
        }

        [HttpGet("GetQuestions/{userId}")]
        public async Task<IActionResult> GetQuestions(long userId, CancellationToken cancellationToken)
        {
            try
            {
                var questions = await _futoshikiService.GetActiveQuestionsAsync(userId, cancellationToken);

                if (questions == null || questions.Count == 0)
                {
                    return Ok(new ApiResponseDto
                    {
                        Success = false,
                        Message = "No Any Game More!"
                    });
                }

                return Ok(new ApiResponseDto
                {
                    Success = true,
                    Message = "Questions fetched successfully.",
                    Data = questions
                });
            }
            catch (OperationCanceledException)
            {
                return StatusCode(499, new ApiResponseDto { Success = false, Message = "Request canceled." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseDto { Success = false, Message = "Error fetching questions: " + ex.Message });
            }
        }

        [HttpPost("SaveAnswer")]
        public async Task<IActionResult> SaveAnswer([FromBody] SaveFutoshikiAnswerDto model, CancellationToken cancellationToken)
        {
            if (model == null || model.UserId <= 0)
            {
                return BadRequest(new ApiResponseDto { Success = false, Message = "Invalid request payload." });
            }

            try
            {
                var response = await _futoshikiService.ValidateAndSaveAnswerAsync(model, cancellationToken);
                return Ok(response);
            }
            catch (OperationCanceledException)
            {
                return StatusCode(499, new ApiResponseDto { Success = false, Message = "Request canceled by client." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseDto { Success = false, Message = "Error saving answer: " + ex.Message });
            }
        }

        [HttpPost("SkipQuestion")]
        public async Task<IActionResult> SkipQuestion([FromBody] SkipFutoshikiDto model, CancellationToken cancellationToken)
        {
            if (model == null || model.UserId <= 0)
            {
                return BadRequest(new ApiResponseDto { Success = false, Message = "Invalid request payload." });
            }

            try
            {
                var response = await _futoshikiService.SkipQuestionAsync(model, cancellationToken);
                return Ok(response);
            }
            catch (OperationCanceledException)
            {
                return StatusCode(499, new ApiResponseDto { Success = false, Message = "Request canceled by client." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseDto { Success = false, Message = "Error skipping question: " + ex.Message });
            }
        }
    }
}