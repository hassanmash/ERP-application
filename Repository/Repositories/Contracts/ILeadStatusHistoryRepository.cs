using Domain.Entities;

namespace Infrastructure.Repositories.Contracts
{
    public interface ILeadStatusHistoryRepository
    {
        void Add(LeadStatusHistory history);

        Task SaveChangesAsync();
    }
}
