using Domain.Entities;
using Infrastructure.Contexts;
using Infrastructure.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Implementations
{
    public class RoleRepository(SalesErpDbContext db) : IRoleRepository
    {
        private readonly SalesErpDbContext _db = db;

        public void Add(Role role)
            => _db.Roles.Add(role);

        public async Task<Role?> GetByIdAsync(Guid id)
            => await _db.Roles.Include(r => r.Users).FirstOrDefaultAsync(r => r.Id == id);

        public async Task<List<Role>> GetAllByOrganizationAsync()
            // Relies on the global query filter (scoped via ICurrentTenantService)
            // to restrict to the caller's own organization — no explicit
            // organizationId parameter by design, see interface XML doc.
            => await _db.Roles.Include(r => r.Users).OrderBy(r => r.Name).ToListAsync();

        public async Task<bool> NameExistsInOrganizationAsync(string name)
            => await _db.Roles.AnyAsync(r => r.Name == name);

        public void Remove(Role role)
            => _db.Roles.Remove(role);

        public async Task SaveChangesAsync()
            => await _db.SaveChangesAsync();
    }
}
