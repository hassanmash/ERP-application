using Domain.Entities;

namespace Infrastructure.Repositories.Contracts
{
    public interface IRoleRepository
    {
        void Add(Role role);
        Task<Role?> GetByIdAsync(Guid id);
        Task<List<Role>> GetAllByOrganizationAsync();
        Task<bool> NameExistsInOrganizationAsync(string name);
        void Remove(Role role);
        Task SaveChangesAsync();
    }
}
