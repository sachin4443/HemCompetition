using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HemCompetition.API.DTOs;

namespace HemCompetition.API.Services
{
    public interface IFutoshikiService
    {
        Task<List<FutoshikiQuestionDto>> GetActiveQuestionsAsync(long userId, CancellationToken cancellationToken);
        Task<ApiResponseDto> ValidateAndSaveAnswerAsync(SaveFutoshikiAnswerDto dto, CancellationToken cancellationToken);
        Task<ApiResponseDto> SkipQuestionAsync(SkipFutoshikiDto dto, CancellationToken cancellationToken);
    }
}