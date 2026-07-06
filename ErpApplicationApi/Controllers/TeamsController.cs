using Application.Common;
using Application.DTOs.OrgAdmin.Teams;
using Application.Services.OrgAdmin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ErpApplicationApi.Controllers
{
    [ApiController]
    [Route("api/org-admin/teams")]
    [Authorize]
    public class TeamsController(ITeamService teamService) : ControllerBase
    {
        private readonly ITeamService _teamService = teamService;

        [HttpGet]
        public async Task<ActionResult<List<TeamResponse>>> GetAll()
        {
            var result = await _teamService.GetAllAsync();
            return result.ToActionResult(this);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<TeamResponse>> GetById(Guid id)
        {
            var result = await _teamService.GetByIdAsync(id);
            return result.ToActionResult(this);
        }

        [HttpPost]
        [Authorize(Policy = "OrgAdminOnly")]
        public async Task<ActionResult<TeamResponse>> Create([FromBody] CreateTeamRequest request)
        {
            var result = await _teamService.CreateAsync(request);
            return result.ToActionResult(this);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Policy = "OrgAdminOnly")]
        public async Task<ActionResult<TeamResponse>> Update(Guid id, [FromBody] UpdateTeamRequest request)
        {
            var result = await _teamService.UpdateAsync(id, request);
            return result.ToActionResult(this);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Policy = "OrgAdminOnly")]
        public async Task<ActionResult<string>> Delete(Guid id)
        {
            var result = await _teamService.DeleteAsync(id);
            return result.ToActionResult(this);
        }
    }
}
