using Domain.Entities;
using Infrastructure.Contexts;
using Infrastructure.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Implementations
{
    public class TeamRepository(SalesErpDbContext db) : ITeamRepository
    {
        private readonly SalesErpDbContext _db = db;

        public void Add(Team team)
            => _db.Teams.Add(team);

        public async Task<Team?> GetByIdAsync(Guid id)
            => await _db.Teams
                .Include(t => t.LeadUser)
                .Include(t => t.Members)
                .FirstOrDefaultAsync(t => t.Id == id);

        public async Task<List<Team>> GetAllByOrganizationAsync()
            => await _db.Teams
                .Include(t => t.LeadUser)
                .Include(t => t.Members)
                .OrderBy(t => t.Name)
                .ToListAsync();

        public async Task<bool> NameExistsInOrganizationAsync(string name)
            => await _db.Teams.AnyAsync(t => t.Name == name);

        public void Remove(Team team)
            => _db.Teams.Remove(team);

        public async Task SaveChangesAsync()
            => await _db.SaveChangesAsync();
    }
}
