using Domain.Enum;
using Infrastructure.Contexts;
using Infrastructure.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Implementations
{
    public class DashboardRepository(SalesErpDbContext context) : IDashboardRepository
    {
        private readonly SalesErpDbContext _context = context;

        public async Task<int> GetTotalLeadCountAsync()
        {
            return await _context.Leads.CountAsync();
        }

        public async Task<int> GetLeadCountByStatusAsync(LeadStatus status)
        {
            return await _context.Leads
                .CountAsync(x => x.Status == status);
        }

        public async Task<int> GetTodaysActivityCountAsync()
        {
            var today = DateTime.UtcNow.Date;

            return await _context.Activities
                .CountAsync(x => x.ActivityDateTime.Date == today);
        }

        public async Task<int> GetOverdueActivityCountAsync()
        {
            return await _context.Activities
                .CountAsync(x =>
                    x.ActivityDateTime < DateTime.UtcNow);
        }

        public async Task<List<(LeadStatus Status, int Count)>> GetLeadStatusSummaryAsync()
        {
            return await _context.Leads
                .GroupBy(x => x.Status)
                .Select(g => new ValueTuple<LeadStatus, int>(
                    g.Key,
                    g.Count()))
                .ToListAsync();
        }

        public async Task<List<(string Source, int Count)>> GetLeadSourceSummaryAsync()
        {
            return await _context.Leads
                .GroupBy(x => x.Source)
                .Select(g => new ValueTuple<string, int>(
                    g.Key,
                    g.Count()))
                .ToListAsync();
        }
    }
}
