using Application.Common;
using Application.DTOs.OrgAdmin.Users;
using Application.Services.OrgAdmin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ErpApplicationApi.Controllers
{
    [ApiController]
    [Route("api/org-admin/users")]
    [Authorize]
    public class UsersController(IUserService userService) : ControllerBase
    {
        private readonly IUserService _userService = userService;

        [HttpGet]
        public async Task<ActionResult<List<UserResponse>>> GetAll()
        {
            var result = await _userService.GetAllAsync();
            return result.ToActionResult(this);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<UserResponse>> GetById(Guid id)
        {
            var result = await _userService.GetByIdAsync(id);
            return result.ToActionResult(this);
        }

        [HttpPost]
        [Authorize(Policy = "OrgAdminOnly")]
        public async Task<ActionResult<UserResponse>> Create([FromBody] CreateUserRequest request)
        {
            var result = await _userService.CreateAsync(request);
            return result.ToActionResult(this);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Policy = "OrgAdminOnly")]
        public async Task<ActionResult<UserResponse>> Update(Guid id, [FromBody] UpdateUserRequest request)
        {
            var result = await _userService.UpdateAsync(id, request);
            return result.ToActionResult(this);
        }
    }
}
