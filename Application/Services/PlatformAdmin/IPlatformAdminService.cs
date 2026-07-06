using Application.Common;
using Application.DTOs.PlatformAdmin;

namespace Application.Services.PlatformAdmin
{
    public interface IPlatformAdminService
    {
        Task<ServiceResult<OrganizationResponse>> CreateOrganizationAsync(CreateOrganizationRequest request);
        Task<ServiceResult<List<OrganizationResponse>>> GetAllOrganizationsAsync();

        Task<ServiceResult<OrganizationResponse>> GetOrganizationByIdAsync(Guid id);

        Task<ServiceResult<OrganizationResponse>> UpdateStatusAsync(Guid id, UpdateOrganizationStatusRequest request);

        Task<ServiceResult<OrganizationResponse>> UpdateModulesAsync(Guid id, UpdateOrganizationModulesRequest request);
    }
}
