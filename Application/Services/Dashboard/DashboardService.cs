using Application.Common;
using Application.DTOs.Dashboard;
using Domain.Enum;
using Infrastructure.Repositories.Contracts;

namespace Application.Services.Dashboard
{
    public class DashboardService(IDashboardRepository dashboardRepository) : IDashboardService
    {
        private readonly IDashboardRepository _dashboardRepository = dashboardRepository;

        public async Task<ServiceResult<DashboardSummaryResponse>> GetSummaryAsync()
        {
            var response = new DashboardSummaryResponse
            {
                TotalLeads = await _dashboardRepository.GetTotalLeadCountAsync(),

                NewLeads = await _dashboardRepository.GetLeadCountByStatusAsync(LeadStatus.New),

                QualifiedLeads = await _dashboardRepository.GetLeadCountByStatusAsync(LeadStatus.SiteVisitCompleted),

                WonLeads = await _dashboardRepository.GetLeadCountByStatusAsync(LeadStatus.Won),

                LostLeads = await _dashboardRepository.GetLeadCountByStatusAsync(LeadStatus.Lost),

                TodaysActivities = await _dashboardRepository.GetTodaysActivityCountAsync(),

                OverdueActivities = await _dashboardRepository.GetOverdueActivityCountAsync()
            };

            return ServiceResult<DashboardSummaryResponse>.Success(response);
        }

        public async Task<ServiceResult<List<LeadStatusSummaryResponse>>> GetLeadStatusSummaryAsync()
        {
            var summary = await _dashboardRepository.GetLeadStatusSummaryAsync();

            var response = summary
                .Select(x => new LeadStatusSummaryResponse
                {
                    Status = x.Status.ToString(),
                    Count = x.Count
                })
                .ToList();

            return ServiceResult<List<LeadStatusSummaryResponse>>.Success(response);
        }

        public async Task<ServiceResult<List<LeadSourceSummaryResponse>>> GetLeadSourceSummaryAsync()
        {
            var summary = await _dashboardRepository.GetLeadSourceSummaryAsync();

            var response = summary
                .Select(x => new LeadSourceSummaryResponse
                {
                    Source = x.Source,
                    Count = x.Count
                })
                .OrderByDescending(x => x.Count)
                .ToList();

            return ServiceResult<List<LeadSourceSummaryResponse>>.Success(response);
        }
    }
}
