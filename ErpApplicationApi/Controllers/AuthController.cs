using Application.DTOs.Auth;
using Application.Services.Auth;
using Microsoft.AspNetCore.Mvc;
using Application.Common;

namespace ErpApplicationApi.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        private readonly IAuthService _authService = authService;

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
        {
            var result = await _authService.LoginAsync(request);
            return result.ToActionResult(this);
        }
    }
}
