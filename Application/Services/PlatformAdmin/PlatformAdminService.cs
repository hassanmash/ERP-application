using Application.Common;
using Application.Constants;
using Application.DTOs.PlatformAdmin;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Contexts;
using Infrastructure.Contracts;
using Infrastructure.Repositories.Contracts;
using Infrastructure.Repositories.Implementations;

namespace Application.Services.PlatformAdmin
{
    public class PlatformAdminService(
        IOrganizationRepository organizationRepository,
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IMapper mapper,
        IRoleRepository roleRepository,
        SalesErpDbContext db) : IPlatformAdminService
    {
        private readonly IOrganizationRepository _organizationRepository = organizationRepository;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IPasswordHasher _passwordHasher = passwordHasher;
        private readonly IMapper _mapper = mapper;
        private readonly IRoleRepository _roleRepository = roleRepository;

        // Injected solely to own the cross-repository transaction boundary for
        // this multi-table operation (org + modules + admin user). Repositories
        // still own all actual reads/writes — this is orchestration, not data
        // access, which is why it's justified here per our agreed split: single-
        // table operations transact inside the Repository, multi-table ones
        // transact at the Service boundary.
        private readonly SalesErpDbContext _db = db;

        public async Task<ServiceResult<OrganizationResponse>> CreateOrganizationAsync(CreateOrganizationRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name) ||
                string.IsNullOrWhiteSpace(request.OrgCode) ||
                string.IsNullOrWhiteSpace(request.AdminEmail) ||
                string.IsNullOrWhiteSpace(request.AdminPassword) ||
                string.IsNullOrWhiteSpace(request.AdminName))
            {
                return ServiceResult<OrganizationResponse>.Failure(
                    "Name, OrgCode, AdminName, AdminEmail, and AdminPassword are all required.",
                    ServiceResultStatus.BadRequest);
            }

            var normalizedOrgCode = request.OrgCode.Trim().ToUpperInvariant();
            var normalizedAdminEmail = request.AdminEmail.Trim().ToLowerInvariant();

            if (await _organizationRepository.OrgCodeExistsAsync(normalizedOrgCode))
            {
                return ServiceResult<OrganizationResponse>.Failure(
                    $"Organization code '{normalizedOrgCode}' is already in use.", ServiceResultStatus.Conflict);
            }

            if (await _userRepository.EmailExistsIgnoringTenantAsync(normalizedAdminEmail))
            {
                return ServiceResult<OrganizationResponse>.Failure(
                    $"Email '{normalizedAdminEmail}' is already registered.", ServiceResultStatus.Conflict);
            }

            await using var transaction = await _db.Database.BeginTransactionAsync();

            try
            {
                var organization = _mapper.Map<Organization>(request);
                organization.Id = Guid.NewGuid();
                organization.OrgCode = normalizedOrgCode;
                organization.Status = OrganizationStatus.Active;
                organization.CreatedAt = DateTime.UtcNow;
                _organizationRepository.Add(organization);

                var distinctModules = request.EnabledModules.Distinct().ToList();
                foreach (var moduleName in distinctModules)
                {
                    _organizationRepository.AddModule(new OrgModule
                    {
                        Id = Guid.NewGuid(),
                        OrganizationId = organization.Id,
                        ModuleName = moduleName,
                        IsEnabled = true
                    });
                }

                var adminUser = new User
                {
                    Id = Guid.NewGuid(),
                    OrganizationId = organization.Id,
                    Name = request.AdminName.Trim(),
                    Email = normalizedAdminEmail,
                    PasswordHash = _passwordHasher.Hash(request.AdminPassword),
                    IsPlatformAdmin = false,
                    IsOrgAdmin = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };
                _userRepository.Add(adminUser);

                // Seed the brief's suggested default roles so the org is
                // immediately usable — org admin can edit or delete any of
                // these afterward via the Roles endpoints, nothing here is
                // protected or "system-locked".
                foreach (var (roleName, permissions) in DefaultRoleSeeds.Roles)
                {
                    _roleRepository.Add(new Role
                    {
                        Id = Guid.NewGuid(),
                        OrganizationId = organization.Id,
                        Name = roleName,
                        Permissions = DefaultRoleSeeds.SerializePermissions(permissions),
                        IsSystemDefault = true
                    });
                }

                // One SaveChangesAsync flushes ALL staged changes across every
                // repository used above (Organization, OrgModule, User, Role) —
                // they share the same underlying SalesErpDbContext instance per
                // request, so calling SaveChangesAsync via any one repository
                // commits everyone's pending changes together; the explicit transaction still matters
                // because the two preceding uniqueness checks happened on
                // separate prior queries — without it, a concurrent request
                // could theoretically slip a duplicate orgCode/email in between
                // our check and our insert. The transaction doesn't fully solve
                // that race on its own (would need DB-level unique constraints
                // backing it up, which we have), but it keeps the writes atomic.
                await _organizationRepository.SaveChangesAsync();
                await transaction.CommitAsync();

                var response = _mapper.Map<OrganizationResponse>(organization);
                response.EnabledModules = distinctModules;
                response.AdminUserId = adminUser.Id;
                response.AdminEmail = adminUser.Email;

                return ServiceResult<OrganizationResponse>.Success(response, ServiceResultStatus.Created);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<ServiceResult<List<OrganizationResponse>>> GetAllOrganizationsAsync()
        {
            var organizations = await _organizationRepository.GetAllAsync();
            var responses = organizations.Select(MapToResponse).ToList();
            return ServiceResult<List<OrganizationResponse>>.Success(responses);
        }

        public async Task<ServiceResult<OrganizationResponse>> GetOrganizationByIdAsync(Guid id)
        {
            var organization = await _organizationRepository.GetByIdWithModulesAsync(id);
            if (organization is null)
            {
                return ServiceResult<OrganizationResponse>.Failure(
                    "Organization not found.", ServiceResultStatus.NotFound);
            }

            return ServiceResult<OrganizationResponse>.Success(MapToResponse(organization));
        }

        public async Task<ServiceResult<OrganizationResponse>> UpdateStatusAsync(
            Guid id, UpdateOrganizationStatusRequest request)
        {
            if (!Enum.TryParse<OrganizationStatus>(request.Status, ignoreCase: true, out var newStatus))
            {
                return ServiceResult<OrganizationResponse>.Failure(
                    $"Invalid status '{request.Status}'. Expected 'Active' or 'Suspended'.",
                    ServiceResultStatus.BadRequest);
            }

            var organization = await _organizationRepository.GetByIdWithModulesAsync(id);
            if (organization is null)
            {
                return ServiceResult<OrganizationResponse>.Failure(
                    "Organization not found.", ServiceResultStatus.NotFound);
            }

            organization.Status = newStatus;
            organization.UpdatedAt = DateTime.UtcNow;
            await _organizationRepository.SaveChangesAsync();

            return ServiceResult<OrganizationResponse>.Success(MapToResponse(organization));
        }

        public async Task<ServiceResult<OrganizationResponse>> UpdateModulesAsync(
            Guid id, UpdateOrganizationModulesRequest request)
        {
            var organization = await _organizationRepository.GetByIdWithModulesAsync(id);
            if (organization is null)
            {
                return ServiceResult<OrganizationResponse>.Failure(
                    "Organization not found.", ServiceResultStatus.NotFound);
            }

            var requestedModules = request.EnabledModules.Distinct().ToList();
            var existingModuleNames = organization.OrgModules.Select(m => m.ModuleName).ToHashSet();

            // Remove rows for modules no longer in the requested list.
            foreach (var existingModule in organization.OrgModules.ToList())
            {
                if (!requestedModules.Contains(existingModule.ModuleName))
                {
                    _organizationRepository.RemoveModule(existingModule);
                }
            }

            // Add rows for newly requested modules not already present.
            foreach (var moduleName in requestedModules)
            {
                if (!existingModuleNames.Contains(moduleName))
                {
                    _organizationRepository.AddModule(new OrgModule
                    {
                        Id = Guid.NewGuid(),
                        OrganizationId = organization.Id,
                        ModuleName = moduleName,
                        IsEnabled = true
                    });
                }
            }

            await _organizationRepository.SaveChangesAsync();

            // Re-fetch to get the true post-update state rather than trusting
            // in-memory tracked collections, which can be subtly stale after
            // mixed Add/Remove operations on a navigation collection.
            var updated = await _organizationRepository.GetByIdWithModulesAsync(id);
            return ServiceResult<OrganizationResponse>.Success(MapToResponse(updated!));
        }

        private OrganizationResponse MapToResponse(Organization organization)
        {
            var response = _mapper.Map<OrganizationResponse>(organization);
            response.EnabledModules = organization.OrgModules
                .Where(m => m.IsEnabled)
                .Select(m => m.ModuleName)
                .ToList();
            return response;
        }
    }
}
