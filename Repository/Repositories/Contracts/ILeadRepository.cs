using Domain.Entities;
using Domain.Enum;

namespace Infrastructure.Repositories.Contracts
{
    public interface ILeadRepository
    {
        Task<Lead?> GetByIdAsync(Guid id);

        Task<List<Lead>> GetAllAsync();

        //Task<bool> ExistsAsync(Guid id);

        //Task<User?> GetAssignedUserAsync(Guid userId);
        Task<List<Lead>> GetAccessibleLeadsAsync(DataScope scope, Guid currentUserId);

        void Add(Lead lead);

        void Delete(Lead lead);

        Task SaveChangesAsync();
    }
}
