using Application.Common;
using Application.DTOs.Dashboard;

namespace Application.Services.Dashboard
{
    public interface IDashboardService
    {
        Task<ServiceResult<DashboardSummaryResponse>> GetSummaryAsync();

        Task<ServiceResult<List<LeadStatusSummaryResponse>>> GetLeadStatusSummaryAsync();

        Task<ServiceResult<List<LeadSourceSummaryResponse>>> GetLeadSourceSummaryAsync();
    }
}
