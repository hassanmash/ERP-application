using Domain.Entities;
using Infrastructure.Contexts;
using Infrastructure.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Infrastructure.Repositories.Implementations
{
    public class UserRepository(SalesErpDbContext db) : IUserRepository
    {
        private readonly SalesErpDbContext _db = db;

        public async Task<User?> GetByEmailIgnoringTenantAsync(string email)
        {
            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                // 1. Inject the email securely using standard NpgsqlParameters to prevent SQL injection
                var emailParam = new NpgsqlParameter("email", email);
                await _db.Database.ExecuteSqlRawAsync("SELECT set_config('app.login_attempt_email', @email, true);", emailParam);

                // 2. Execute the lookup (IgnoreQueryFilters bypasses C# filters, PostgreSQL handles RLS)
                var user = await _db.Users
                    .IgnoreQueryFilters()
                    .FirstOrDefaultAsync(u => u.Email == email);

                // Commit the transaction to close the session safely
                await transaction.CommitAsync();


                return user;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
            //return await _db.Users
            //    .IgnoreQueryFilters()
            //    .Include(u => u.Organization)
            //    .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<bool> EmailExistsIgnoringTenantAsync(string email)
            => await _db.Users
                .IgnoreQueryFilters()
                .AnyAsync(u => u.Email == email);

        public async Task<User?> GetByIdAsync(Guid id)
            => await _db.Users.FirstOrDefaultAsync(u => u.Id == id);

        public async Task<List<User>> GetAllByOrganizationAsync()
        => await _db.Users
            .Include(u => u.Team)
            .Include(u => u.Role)
            .OrderBy(u => u.Name)
            .ToListAsync();

        public async Task<bool> EmailExistsInOrganizationAsync(string email)
        => await _db.Users.AnyAsync(u => u.Email == email);

        public async Task<int> CountByRoleIdAsync(Guid roleId)
            => await _db.Users.CountAsync(u => u.RoleId == roleId);

        public async Task<int> CountByTeamIdAsync(Guid teamId)
            => await _db.Users.CountAsync(u => u.TeamId == teamId);


        public void Add(User user)
            => _db.Users.Add(user);

        public async Task SaveChangesAsync()
            => await _db.SaveChangesAsync();
    }
}
