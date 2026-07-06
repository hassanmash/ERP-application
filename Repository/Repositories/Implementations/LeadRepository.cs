using Domain.Entities;
using Domain.Enum;
using Infrastructure.Contexts;
using Infrastructure.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Implementations
{
    public class LeadRepository(SalesErpDbContext db) : ILeadRepository
    {
        private readonly SalesErpDbContext _db = db;

        public async Task<Lead?> GetByIdAsync(Guid id)
            => await _db.Leads
                .Include(l => l.AssignedUser)
                .FirstOrDefaultAsync(l => l.Id == id);

        public async Task<List<Lead>> GetAllAsync()
            => await _db.Leads
                .Include(l => l.AssignedUser)
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync();

        //public async Task<bool> ExistsAsync(Guid id)
        //    => await _db.Leads.AnyAsync(l => l.Id == id);

        //public async Task<User?> GetAssignedUserAsync(Guid userId)
        //    => await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);

        public async Task<List<Lead>> GetAccessibleLeadsAsync(
            DataScope scope,
            Guid currentUserId)
        {
            IQueryable<Lead> query = _db.Leads.Include(l => l.AssignedUser);

            switch (scope)
            {
                case DataScope.Organization:
                    break;

                case DataScope.Own:
                    query = query.Where(l => l.AssignedUserId == currentUserId);
                    break;

                case DataScope.Team:
                    {
                        var teamId = await _db.Users
                            .Where(u => u.Id == currentUserId)
                            .Select(u => u.TeamId)
                            .FirstOrDefaultAsync();

                        if (teamId == null)
                            return [];

                        query =
                            from lead in _db.Leads
                            join user in _db.Users
                                on lead.AssignedUserId equals user.Id
                            where user.TeamId == teamId
                            select lead;

                        break;
                    }
            }

            return await query
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync();
        }

        public void Add(Lead lead)
            => _db.Leads.Add(lead);

        public void Delete(Lead lead)
            => _db.Leads.Remove(lead);

        public async Task SaveChangesAsync()
            => await _db.SaveChangesAsync();
    }
}
