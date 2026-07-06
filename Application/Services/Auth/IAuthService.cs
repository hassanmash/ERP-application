using Application.Common;
using Application.DTOs.Auth;

namespace Application.Services.Auth
{
    public interface IAuthService
    {
        Task<ServiceResult<LoginResponse>> LoginAsync(LoginRequest request);
    }
}
