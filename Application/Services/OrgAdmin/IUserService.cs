using Application.Common;
using Application.DTOs.OrgAdmin.Users;

namespace Application.Services.OrgAdmin
{
    public interface IUserService
    {
        Task<ServiceResult<List<UserResponse>>> GetAllAsync();

        Task<ServiceResult<UserResponse>> GetByIdAsync(Guid id);

        Task<ServiceResult<UserResponse>> CreateAsync(CreateUserRequest request);

        Task<ServiceResult<UserResponse>> UpdateAsync(Guid id, UpdateUserRequest request);
    }
}
