using Application.Common;
using Application.DTOs.Sales;

namespace Application.Services.Sales
{
    public interface IActivityService
    {
        Task<ServiceResult<List<ActivityResponse>>> GetByLeadIdAsync(Guid leadId);

        Task<ServiceResult<ActivityResponse>> GetByIdAsync(Guid id);

        Task<ServiceResult<ActivityResponse>> CreateAsync(CreateActivityRequest request);

        Task<ServiceResult<ActivityResponse>> UpdateAsync(Guid id, UpdateActivityRequest request);

        Task<ServiceResult> DeleteAsync(Guid id);
    }
}
