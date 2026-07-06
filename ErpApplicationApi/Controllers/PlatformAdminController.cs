using Application.DTOs.PlatformAdmin;
using Application.Services.PlatformAdmin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.Common;

namespace ErpApplicationApi.Controllers
{
    [ApiController]
    [Route("api/platform-admin/organizations")]
    [Authorize(Policy = "PlatformAdminOnly")]
    public class PlatformAdminController(IPlatformAdminService platformAdminService) : ControllerBase
    {
        private readonly IPlatformAdminService _platformAdminService = platformAdminService;

        [HttpPost]
        public async Task<ActionResult<OrganizationResponse>> CreateOrganization(
            [FromBody] CreateOrganizationRequest request)
        {
            var result = await _platformAdminService.CreateOrganizationAsync(request);
            return result.ToActionResult(this);
        }

        [HttpGet]
        public async Task<ActionResult<List<OrganizationResponse>>> GetAllOrganizations()
        {
            var result = await _platformAdminService.GetAllOrganizationsAsync();
            return result.ToActionResult(this);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<OrganizationResponse>> GetOrganizationById(Guid id)
        {
            var result = await _platformAdminService.GetOrganizationByIdAsync(id);
            return result.ToActionResult(this);
        }

        [HttpPatch("{id:guid}/status")]
        public async Task<ActionResult<OrganizationResponse>> UpdateStatus(
        Guid id, [FromBody] UpdateOrganizationStatusRequest request)
        {
            var result = await _platformAdminService.UpdateStatusAsync(id, request);
            return result.ToActionResult(this);
        }

        [HttpPut("{id:guid}/modules")]
        public async Task<ActionResult<OrganizationResponse>> UpdateModules(
        Guid id, [FromBody] UpdateOrganizationModulesRequest request)
        {
            var result = await _platformAdminService.UpdateModulesAsync(id, request);
            return result.ToActionResult(this);
        }
    }
}
