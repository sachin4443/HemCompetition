using HemCompetition.API.Data;
using HemCompetition.API.Models;
using Microsoft.EntityFrameworkCore;

namespace HemCompetition.API.Services
{
    public class AuthSessionService : IAuthSessionService
    {
        private readonly AuthDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthSessionService(
            AuthDbContext context,
            IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // =========================================================
        // CREATE NEW SESSION
        // =========================================================

        public async Task<Guid> CreateNewSessionAsync(long userId)
        {
            var newSessionId = Guid.NewGuid();
            var now = DateTime.UtcNow;

            // Existing active session terminate
            var existingSessions = await _context.AuthSessions
                .Where(x =>
                    x.UserId == userId &&
                    x.IsActive)
                .ToListAsync();

            foreach (var existingSession in existingSessions)
            {
                existingSession.IsActive = false;
                existingSession.TerminatedAtUtc = now;
                existingSession.LastActivityUtc = now;
            }

            var session = new AuthSession
            {
                Id = Guid.NewGuid(),

                UserId = userId,

                SessionId = newSessionId,

                CreatedAtUtc = now,

                LastActivityUtc = now,

                LastHeartbeatUtc = now,

                IsActive = true
            };

            _context.AuthSessions.Add(session);

            await _context.SaveChangesAsync();

            return newSessionId;
        }


        // =========================================================
        // GET ACTIVE SESSION BY USER
        // =========================================================

        public async Task<AuthSession?> GetActiveSessionAsync(long userId)
        {
            return await _context.AuthSessions
                .FirstOrDefaultAsync(x =>
                    x.UserId == userId &&
                    x.IsActive);
        }


        // =========================================================
        // GET ACTIVE SESSION BY SESSION ID
        // =========================================================

        public async Task<AuthSession?> GetActiveSessionBySessionIdAsync(
            Guid sessionId)
        {
            return await _context.AuthSessions
                .FirstOrDefaultAsync(x =>
                    x.SessionId == sessionId &&
                    x.IsActive);
        }


        // =========================================================
        // VALIDATE SESSION
        // =========================================================

        public async Task<bool> ValidateSessionAsync(
            long userId,
            Guid sessionId)
        {
            var session = await _context.AuthSessions
                .FirstOrDefaultAsync(x =>
                    x.UserId == userId &&
                    x.SessionId == sessionId &&
                    x.IsActive);

            if (session == null)
                return false;

            if (session.LastHeartbeatUtc == null)
                return false;

            var timeoutSeconds =
                _configuration.GetValue<int>(
                    "SessionSettings:HeartbeatTimeoutSeconds");

            if (session.LastHeartbeatUtc <
                DateTime.UtcNow.AddSeconds(-timeoutSeconds))
            {
                session.IsActive = false;
                session.TerminatedAtUtc = DateTime.UtcNow;
                session.LastActivityUtc = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return false;
            }

            session.LastActivityUtc = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return true;
        }


        // =========================================================
        // UPDATE HEARTBEAT
        // =========================================================

        public async Task<bool> UpdateHeartbeatAsync(
            long userId,
            Guid sessionId)
        {
            var session = await _context.AuthSessions
                .FirstOrDefaultAsync(x =>
                    x.UserId == userId &&
                    x.SessionId == sessionId &&
                    x.IsActive);

            if (session == null)
                return false;

            var now = DateTime.UtcNow;

            session.LastHeartbeatUtc = now;
            session.LastActivityUtc = now;

            await _context.SaveChangesAsync();

            return true;
        }


        // =========================================================
        // TERMINATE SESSION
        // =========================================================

        public async Task TerminateSessionAsync(long userId)
        {
            var sessions = await _context.AuthSessions
                .Where(x =>
                    x.UserId == userId &&
                    x.IsActive)
                .ToListAsync();

            if (!sessions.Any())
                return;

            var now = DateTime.UtcNow;

            foreach (var session in sessions)
            {
                session.IsActive = false;
                session.TerminatedAtUtc = now;
                session.LastActivityUtc = now;
            }

            await _context.SaveChangesAsync();
        }
    }
}