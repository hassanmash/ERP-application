using Domain.Entities;
using Infrastructure.Contexts;
using Infrastructure.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Implementations
{
    public class OrganizationRepository(SalesErpDbContext db) : IOrganizationRepository
    {
        private readonly SalesErpDbContext _db = db;

        public async Task<bool> OrgCodeExistsAsync(string orgCode)
            => await _db.Organizations.AnyAsync(o => o.OrgCode == orgCode);

        public async Task<Organization?> GetByIdAsync(Guid id)
            => await _db.Organizations.FirstOrDefaultAsync(o => o.Id == id);

        public async Task<Organization?> GetByIdWithModulesAsync(Guid id)
        => await _db.Organizations
            .Include(o => o.OrgModules)
            .FirstOrDefaultAsync(o => o.Id == id);

        public async Task<List<Organization>> GetAllAsync()
        => await _db.Organizations
            .Include(o => o.OrgModules)
            .OrderBy(o => o.Name)
            .ToListAsync();

        public void Add(Organization organization)
            => _db.Organizations.Add(organization);

        public void AddModule(OrgModule module)
            => _db.OrgModules.Add(module);

        public void RemoveModule(OrgModule module)
        => _db.OrgModules.Remove(module);

        public async Task SaveChangesAsync()
            => await _db.SaveChangesAsync();
    }
}
