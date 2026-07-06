using Application.Common;
using Application.DTOs.OrgAdmin.Users;
using Domain.Entities;
using Infrastructure.Contracts;
using Infrastructure.Repositories.Contracts;

namespace Application.Services.OrgAdmin
{
    public class UserService(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        ITeamRepository teamRepository,
        IPasswordHasher passwordHasher,
        ICurrentTenantService tenant) : IUserService
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IRoleRepository _roleRepository = roleRepository;
        private readonly ITeamRepository _teamRepository = teamRepository;
        private readonly IPasswordHasher _passwordHasher = passwordHasher;
        private readonly ICurrentTenantService _tenant = tenant;

        public async Task<ServiceResult<List<UserResponse>>> GetAllAsync()
        {
            var users = await _userRepository.GetAllByOrganizationAsync();
            return ServiceResult<List<UserResponse>>.Success(users.Select(MapToResponse).ToList());
        }

        public async Task<ServiceResult<UserResponse>> GetByIdAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user is null)
            {
                return ServiceResult<UserResponse>.Failure("User not found.", ServiceResultStatus.NotFound);
            }

            return ServiceResult<UserResponse>.Success(MapToResponse(user));
        }

        public async Task<ServiceResult<UserResponse>> CreateAsync(CreateUserRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name) ||
                string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Password))
            {
                return ServiceResult<UserResponse>.Failure(
                    "Name, Email, and Password are required.", ServiceResultStatus.BadRequest);
            }

            if (_tenant.OrganizationId is null)
            {
                return ServiceResult<UserResponse>.Failure(
                    "Users must be created within an organization context.", ServiceResultStatus.BadRequest);
            }

            var normalizedEmail = request.Email.Trim().ToLowerInvariant();

            // Email uniqueness within this org — uses the tenant-scoped check
            // (global query filter active), not the cross-org check used at login.
            if (await _userRepository.EmailExistsInOrganizationAsync(normalizedEmail))
            {
                return ServiceResult<UserResponse>.Failure(
                    $"Email '{normalizedEmail}' is already in use within this organization.",
                    ServiceResultStatus.Conflict);
            }

            // Validate Team and Role if provided — both must belong to the same org.
            // The query filter makes cross-org results structurally impossible here,
            // but we still validate existence explicitly to return a clear 400
            // rather than an FK violation from the DB.
            if (request.TeamId.HasValue)
            {
                var team = await _teamRepository.GetByIdAsync(request.TeamId.Value);
                if (team is null)
                {
                    return ServiceResult<UserResponse>.Failure(
                        "The specified team does not exist.", ServiceResultStatus.BadRequest);
                }
            }

            if (request.RoleId.HasValue)
            {
                var role = await _roleRepository.GetByIdAsync(request.RoleId.Value);
                if (role is null)
                {
                    return ServiceResult<UserResponse>.Failure(
                        "The specified role does not exist.", ServiceResultStatus.BadRequest);
                }
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                OrganizationId = _tenant.OrganizationId.Value,
                Name = request.Name.Trim(),
                Email = normalizedEmail,
                PasswordHash = _passwordHasher.Hash(request.Password),
                TeamId = request.TeamId,
                RoleId = request.RoleId,
                IsPlatformAdmin = false,
                IsOrgAdmin = false,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _userRepository.Add(user);
            await _userRepository.SaveChangesAsync();

            // Re-fetch to get Team and Role navigation properties loaded for the response.
            var created = await _userRepository.GetByIdAsync(user.Id);
            return ServiceResult<UserResponse>.Success(MapToResponse(created!), ServiceResultStatus.Created);
        }

        public async Task<ServiceResult<UserResponse>> UpdateAsync(Guid id, UpdateUserRequest request)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user is null)
            {
                return ServiceResult<UserResponse>.Failure("User not found.", ServiceResultStatus.NotFound);
            }

            if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Email))
            {
                return ServiceResult<UserResponse>.Failure(
                    "Name and Email are required.", ServiceResultStatus.BadRequest);
            }

            var normalizedEmail = request.Email.Trim().ToLowerInvariant();

            // Only check email uniqueness if it actually changed — renaming to
            // the same email is a no-op, not a conflict.
            if (!string.Equals(user.Email, normalizedEmail, StringComparison.Ordinal)
                && await _userRepository.EmailExistsInOrganizationAsync(normalizedEmail))
            {
                return ServiceResult<UserResponse>.Failure(
                    $"Email '{normalizedEmail}' is already in use within this organization.",
                    ServiceResultStatus.Conflict);
            }

            if (request.TeamId.HasValue)
            {
                var team = await _teamRepository.GetByIdAsync(request.TeamId.Value);
                if (team is null)
                {
                    return ServiceResult<UserResponse>.Failure(
                        "The specified team does not exist.", ServiceResultStatus.BadRequest);
                }
            }

            if (request.RoleId.HasValue)
            {
                var role = await _roleRepository.GetByIdAsync(request.RoleId.Value);
                if (role is null)
                {
                    return ServiceResult<UserResponse>.Failure(
                        "The specified role does not exist.", ServiceResultStatus.BadRequest);
                }
            }

            user.Name = request.Name.Trim();
            user.Email = normalizedEmail;
            user.IsActive = request.IsActive;
            user.TeamId = request.TeamId;   // null = unassign from team
            user.RoleId = request.RoleId;   // null = unassign from role

            await _userRepository.SaveChangesAsync();

            var updated = await _userRepository.GetByIdAsync(id);
            return ServiceResult<UserResponse>.Success(MapToResponse(updated!));
        }

        private static UserResponse MapToResponse(User user) => new()
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            IsActive = user.IsActive,
            IsOrgAdmin = user.IsOrgAdmin,
            TeamId = user.TeamId,
            TeamName = user.Team?.Name,
            RoleId = user.RoleId,
            RoleName = user.Role?.Name,
            CreatedAt = user.CreatedAt
        };
    }
}
