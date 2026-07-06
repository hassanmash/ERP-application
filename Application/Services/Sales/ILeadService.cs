using Application.Common;
using Application.DTOs.Sales;

namespace Application.Services.Sales
{
    public interface ILeadService
    {
        Task<ServiceResult<List<LeadResponse>>> GetAllAsync();

        Task<ServiceResult<LeadResponse>> GetByIdAsync(Guid id);

        Task<ServiceResult<LeadResponse>> CreateAsync(CreateLeadRequest request);

        Task<ServiceResult<LeadResponse>> UpdateAsync(Guid id, UpdateLeadRequest request);

        Task<ServiceResult> ChangeStatusAsync(Guid id, ChangeLeadStatusRequest request);

        Task<ServiceResult> DeleteAsync(Guid id);
    }
}
