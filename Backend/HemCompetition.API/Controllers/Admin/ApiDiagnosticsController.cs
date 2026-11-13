using HemCompetition.API.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HemCompetition.API.Controllers
{
    [ApiController]
    [Route("api/admin-dashboard/diagnostics")]
    [Authorize(Policy = HemCompetition.API.Security.Policies.Admin)]
    [HemCompetition.API.Security.RequirePermission(HemCompetition.API.Security.Permissions.DiagnosticsView)]
    public class ApiDiagnosticsController : ControllerBase
    {
        private readonly IApiDiagnosticsService _service;

        public ApiDiagnosticsController(
            IApiDiagnosticsService service)
        {
            _service = service;
        }


        // =====================================================
        // GET SYSTEM DIAGNOSTICS
        // =====================================================

        [HttpGet]
        public IActionResult Get()
        {
            var summary =
                _service.GetSummary();

            return Ok(new
            {
                success = true,
                data = summary
            });
        }
    }
}