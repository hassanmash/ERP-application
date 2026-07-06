using Application.Common;
using Application.DTOs.OrgAdmin.Roles;

namespace Application.Services.OrgAdmin
{
    public interface IRoleService
    {
        Task<ServiceResult<List<RoleResponse>>> GetAllAsync();
        Task<ServiceResult<RoleResponse>> GetByIdAsync(Guid id);
        Task<ServiceResult<RoleResponse>> CreateAsync(CreateRoleRequest request);
        Task<ServiceResult<RoleResponse>> UpdateAsync(Guid id, UpdateRoleRequest request);
        Task<ServiceResult<string>> DeleteAsync(Guid id);
    }
}
