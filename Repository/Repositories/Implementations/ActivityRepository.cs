using Domain.Entities;
using Infrastructure.Contexts;
using Infrastructure.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Implementations
{
    public class ActivityRepository(SalesErpDbContext db) : IActivityRepository
    {
        private readonly SalesErpDbContext _db = db;

        public async Task<Activity?> GetByIdAsync(Guid id)
            => await _db.Activities
                .Include(a => a.CreatedByUser)
                .Include(a => a.Lead)
                .FirstOrDefaultAsync(a => a.Id == id);

        public async Task<List<Activity>> GetByLeadIdAsync(Guid leadId)
            => await _db.Activities
                .Include(a => a.CreatedByUser)
                .Where(a => a.LeadId == leadId)
                .OrderByDescending(a => a.ActivityDateTime)
                .ToListAsync();

        //public async Task<Lead?> GetLeadAsync(Guid leadId)
        //    => await _db.Leads.FirstOrDefaultAsync(l => l.Id == leadId);

        public void Add(Activity activity)
            => _db.Activities.Add(activity);

        public void Delete(Activity activity)
            => _db.Activities.Remove(activity);

        public async Task SaveChangesAsync()
            => await _db.SaveChangesAsync();
    }
}
