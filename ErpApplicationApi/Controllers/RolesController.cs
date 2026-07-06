using Application.Services.OrgAdmin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.Common;
using Application.DTOs.OrgAdmin.Roles;

namespace ErpApplicationApi.Controllers
{
    [ApiController]
    [Route("api/org-admin/roles")]
    [Authorize]
    public class RolesController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RolesController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet]
        public async Task<ActionResult<List<RoleResponse>>> GetAll()
        {
            var result = await _roleService.GetAllAsync();
            return result.ToActionResult(this);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<RoleResponse>> GetById(Guid id)
        {
            var result = await _roleService.GetByIdAsync(id);
            return result.ToActionResult(this);
        }

        [HttpPost]
        [Authorize(Policy = "OrgAdminOnly")]
        public async Task<ActionResult<RoleResponse>> Create([FromBody] CreateRoleRequest request)
        {
            var result = await _roleService.CreateAsync(request);
            return result.ToActionResult(this);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Policy = "OrgAdminOnly")]
        public async Task<ActionResult<RoleResponse>> Update(Guid id, [FromBody] UpdateRoleRequest request)
        {
            var result = await _roleService.UpdateAsync(id, request);
            return result.ToActionResult(this);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Policy = "OrgAdminOnly")]
        public async Task<ActionResult<string>> Delete(Guid id)
        {
            var result = await _roleService.DeleteAsync(id);
            return result.ToActionResult(this);
        }

    }
}
