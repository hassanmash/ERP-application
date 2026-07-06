using Application.Common;
using Application.DTOs.Sales;
using Application.Services.Sales;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ErpApplicationApi.Controllers
{
    [ApiController]
    [Route("api/sales/activities")]
    [Authorize]
    public class ActivitiesController(IActivityService activityService) : ControllerBase
    {
        private readonly IActivityService _activityService = activityService;

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ActivityResponse>> GetById(Guid id)
        {
            var result = await _activityService.GetByIdAsync(id);
            return result.ToActionResult(this);
        }

        [HttpGet("lead/{leadId:guid}")]
        public async Task<ActionResult<List<ActivityResponse>>> GetByLead(Guid leadId)
        {
            var result = await _activityService.GetByLeadIdAsync(leadId);
            return result.ToActionResult(this);
        }

        [HttpPost]
        public async Task<ActionResult<ActivityResponse>> Create(
            [FromBody] CreateActivityRequest request)
        {
            var result = await _activityService.CreateAsync(request);
            return result.ToActionResult(this);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<ActivityResponse>> Update(
            Guid id,
            [FromBody] UpdateActivityRequest request)
        {
            var result = await _activityService.UpdateAsync(id, request);
            return result.ToActionResult(this);
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var result = await _activityService.DeleteAsync(id);
            return result.ToActionResult(this);
        }
    }
}
