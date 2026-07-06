using Domain.Enum;

namespace Infrastructure.Repositories.Contracts
{
    public interface IDashboardRepository
    {
        Task<int> GetTotalLeadCountAsync();

        Task<int> GetLeadCountByStatusAsync(LeadStatus status);

        Task<int> GetTodaysActivityCountAsync();

        Task<int> GetOverdueActivityCountAsync();

        Task<List<(LeadStatus Status, int Count)>> GetLeadStatusSummaryAsync();

        Task<List<(string Source, int Count)>> GetLeadSourceSummaryAsync();
    }
}
