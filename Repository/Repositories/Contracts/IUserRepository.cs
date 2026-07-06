using Domain.Entities;

namespace Infrastructure.Repositories.Contracts
{
    /// /// <summary>
    /// Pure data access. No business rules live here.
    /// Methods suffixed "IgnoringTenant" explicitly bypass the global query
    /// filter — only valid at login time, where no tenant context exists yet.
    /// All other methods rely on the filter being active via ICurrentTenantService.
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>Cross-org lookup — only valid at login time before tenant
        /// context is established. Use GetByIdAsync for all other lookups.</summary>
        Task<User?> GetByEmailIgnoringTenantAsync(string email);
        Task<bool> EmailExistsIgnoringTenantAsync(string email);

        /// <summary>Org-scoped — relies on the global query filter.
        /// Used for uniqueness checks within an org during user create/update.</summary>
        Task<bool> EmailExistsInOrganizationAsync(string email);
        Task<User?> GetByIdAsync(Guid id);
        Task<List<User>> GetAllByOrganizationAsync();
        /// <summary>Used when deleting a Role, to report how many users will be
        /// left with RoleId=null (SetNull on delete) so the caller can surface
        /// that to the org admin rather than it happening silently.</summary>
        Task<int> CountByRoleIdAsync(Guid roleId);
        Task<int> CountByTeamIdAsync(Guid teamId);
        void Add(User user);
        Task SaveChangesAsync();
    }
}
