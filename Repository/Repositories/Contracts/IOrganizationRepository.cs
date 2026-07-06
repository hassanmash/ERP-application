using Domain.Entities;

namespace Infrastructure.Repositories.Contracts
{
    public interface IOrganizationRepository
    {
        Task<bool> OrgCodeExistsAsync(string orgCode);
        Task<Organization?> GetByIdAsync(Guid id);
        
        /// <summary>Includes OrgModules — needed for both the detail view and
        /// the module-replace operation, which has to see current rows to
        /// diff against the incoming list.</summary>
        Task<Organization?> GetByIdWithModulesAsync(Guid id);
        Task<List<Organization>> GetAllAsync();
        void Add(Organization organization);
        void AddModule(OrgModule module);
        void RemoveModule(OrgModule module);
        Task SaveChangesAsync();
    }
}
