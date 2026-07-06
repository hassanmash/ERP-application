using Domain.Entities;

namespace Infrastructure.Repositories.Contracts
{
    public interface ITeamRepository
    {
        void Add(Team team);

        Task<Team?> GetByIdAsync(Guid id);

        Task<List<Team>> GetAllByOrganizationAsync();

        Task<bool> NameExistsInOrganizationAsync(string name);

        void Remove(Team team);

        Task SaveChangesAsync();
    }
}
