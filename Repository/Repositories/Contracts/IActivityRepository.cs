using Domain.Entities;

namespace Infrastructure.Repositories.Contracts
{
    public interface IActivityRepository
    {
        Task<Activity?> GetByIdAsync(Guid id);

        Task<List<Activity>> GetByLeadIdAsync(Guid leadId);

        //Task<Lead?> GetLeadAsync(Guid leadId);

        void Add(Activity activity);

        void Delete(Activity activity);

        Task SaveChangesAsync();
    }
}
