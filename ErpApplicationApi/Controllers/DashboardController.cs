using Application.Common;
using Application.DTOs.Dashboard;
using Application.Services.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ErpApplicationApi.Controllers
{
    [ApiController]
    [Route("api/dashboard")]
    [Authorize]
    public class DashboardController(IDashboardService dashboardService) : ControllerBase
    {
        private readonly IDashboardService _dashboardService = dashboardService;

        [HttpGet("summary")]
        public async Task<ActionResult<DashboardSummaryResponse>> GetSummary()
        {
            var result = await _dashboardService.GetSummaryAsync();

            return result.ToActionResult(this);
        }

        [HttpGet("lead-status")]
        public async Task<ActionResult<List<LeadStatusSummaryResponse>>> GetLeadStatusSummary()
        {
            var result = await _dashboardService.GetLeadStatusSummaryAsync();

            return result.ToActionResult(this);
        }

        [HttpGet("lead-sources")]
        public async Task<ActionResult<List<LeadSourceSummaryResponse>>> GetLeadSourceSummary()
        {
            var result = await _dashboardService.GetLeadSourceSummaryAsync();

            return result.ToActionResult(this);
        }
    }
}
