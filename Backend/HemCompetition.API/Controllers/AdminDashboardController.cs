using HemCompetition.API.Data;
using HemCompetition.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HemCompetition.API.Controllers
{
    [Route("api/admin-dashboard")]
    [ApiController]
    [Authorize(Policy = "AdminOnly")]
    public class AdminDashboardController : ControllerBase
    {
        private readonly AuthDbContext _context;
        private readonly IApiMetricsService _metricsService;

        public AdminDashboardController(AuthDbContext context, IApiMetricsService metricsService)
        {
            _context = context;
            _metricsService = metricsService;
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            var totalUsers = await _context.AuthUsers.CountAsync();

            var totalActiveUsers = await _context.AuthUsers
                .CountAsync(x => x.IsActive && !x.IsUserDeactivated);

            var totalInactiveUsers = await _context.AuthUsers
                .CountAsync(x => !x.IsActive || x.IsUserDeactivated);

            var currentLoginUsers = await _context.AuthSessions
                .Where(x => x.IsActive)
                .Select(x => x.UserId)
                .Distinct()
                .CountAsync();

            var totalActiveSessions = await _context.AuthSessions
                .CountAsync(x => x.IsActive);

            var totalDisqualifiedUsers = await _context.CompetitionSecurities
                .CountAsync(x => x.Status != null && x.Status.ToUpper() == "DISQUALIFIED");

            var totalUnloggedUsers = Math.Max(totalActiveUsers - currentLoginUsers, 0);

            // Fetch Realtime System & Developer API Logs
            var metrics = _metricsService.GetSummary();

            return Ok(new
            {
                success = true,
                totalUsers,
                currentLoginUsers,
                totalUnloggedUsers,
                totalDisqualifiedUsers,
                totalActiveSessions,
                totalActiveUsers,
                totalInactiveUsers,
                metrics
            });
        }
    }
}