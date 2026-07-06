using Application.Common;
using Application.DTOs.Sales;
using Application.Services.Sales;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ErpApplicationApi.Controllers
{
    [ApiController]
    [Route("api/sales/leads")]
    [Authorize]
    public class LeadsController(ILeadService leadService) : ControllerBase
    {
        private readonly ILeadService _leadService = leadService;

        [HttpGet]
        public async Task<ActionResult<List<LeadResponse>>> GetAll()
        {
            var result = await _leadService.GetAllAsync();
            return result.ToActionResult(this);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<LeadResponse>> GetById(Guid id)
        {
            var result = await _leadService.GetByIdAsync(id);
            return result.ToActionResult(this);
        }

        [HttpPost]
        [Authorize(Policy = "OrgAdminOnly")]
        public async Task<ActionResult<LeadResponse>> Create(
            [FromBody] CreateLeadRequest request)
        {
            var result = await _leadService.CreateAsync(request);
            return result.ToActionResult(this);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Policy = "OrgAdminOnly")]
        public async Task<ActionResult<LeadResponse>> Update(
            Guid id,
            [FromBody] UpdateLeadRequest request)
        {
            var result = await _leadService.UpdateAsync(id, request);
            return result.ToActionResult(this);
        }

        [HttpPatch("{id:guid}/status")]
        public async Task<ActionResult> ChangeStatus(
            Guid id,
            [FromBody] ChangeLeadStatusRequest request)
        {
            var result = await _leadService.ChangeStatusAsync(id, request);
            return result.ToActionResult(this);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Policy = "OrgAdminOnly")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var result = await _leadService.DeleteAsync(id);
            return result.ToActionResult(this);
        }
    }
}
