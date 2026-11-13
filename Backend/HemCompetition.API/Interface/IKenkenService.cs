using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HemCompetition.API.DTOs;

namespace HemCompetition.API.Services
{
    public interface IKenkenService
    {
        Task<List<KenkenQuestionDto>> GetActiveQuestionsAsync(long userId, CancellationToken cancellationToken);
        Task<ApiResponseDto> ValidateAndSaveAnswerAsync(SaveKenkenAnswerDto dto, CancellationToken cancellationToken);
        Task<ApiResponseDto> SkipQuestionAsync(SkipKenkenDto dto, CancellationToken cancellationToken);
    }
}