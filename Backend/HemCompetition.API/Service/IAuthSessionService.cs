using HemCompetition.API.Models;

namespace HemCompetition.API.Services
{
    public interface IAuthSessionService
    {
        Task<Guid> CreateNewSessionAsync(long userId);

        Task TerminateSessionAsync(long userId);

        Task<AuthSession?> GetActiveSessionAsync(long userId);

        Task<AuthSession?> GetActiveSessionBySessionIdAsync(
            Guid sessionId);

        Task<bool> ValidateSessionAsync(
            long userId,
            Guid sessionId);

        Task<bool> UpdateHeartbeatAsync(
            long userId,
            Guid sessionId);
    }
}