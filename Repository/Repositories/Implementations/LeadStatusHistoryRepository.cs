using Domain.Entities;
using Infrastructure.Contexts;
using Infrastructure.Repositories.Contracts;

namespace Infrastructure.Repositories.Implementations
{
    public class LeadStatusHistoryRepository(SalesErpDbContext db)
        : ILeadStatusHistoryRepository
    {
        private readonly SalesErpDbContext _db = db;

        public void Add(LeadStatusHistory history)
            => _db.LeadStatusHistories.Add(history);

        public async Task SaveChangesAsync()
            => await _db.SaveChangesAsync();
    }
}
