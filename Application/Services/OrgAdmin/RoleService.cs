using Application.Common;
using Application.DTOs.OrgAdmin.Roles;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Contracts;
using Infrastructure.Repositories.Contracts;
using System.Text.Json;

namespace Application.Services.OrgAdmin
{
    public class RoleService(IRoleRepository roleRepository, IUserRepository userRepository, ICurrentTenantService tenant) : IRoleService
    {
        private readonly IRoleRepository _roleRepository = roleRepository;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly ICurrentTenantService _tenant = tenant;

        public async Task<ServiceResult<List<RoleResponse>>> GetAllAsync()
        {
            var roles = await _roleRepository.GetAllByOrganizationAsync();
            var responses = roles.Select(MapToResponse).ToList();
            return ServiceResult<List<RoleResponse>>.Success(responses);
        }

        public async Task<ServiceResult<RoleResponse>> GetByIdAsync(Guid id)
        {
            var role = await _roleRepository.GetByIdAsync(id);
            if (role is null)
            {
                return ServiceResult<RoleResponse>.Failure("Role not found.", ServiceResultStatus.NotFound);
            }

            return ServiceResult<RoleResponse>.Success(MapToResponse(role));
        }

        public async Task<ServiceResult<RoleResponse>> CreateAsync(CreateRoleRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return ServiceResult<RoleResponse>.Failure("Role name is required.", ServiceResultStatus.BadRequest);
            }

            var trimmedName = request.Name.Trim();

            if (await _roleRepository.NameExistsInOrganizationAsync(trimmedName))
            {
                return ServiceResult<RoleResponse>.Failure(
                    $"A role named '{trimmedName}' already exists in this organization.",
                    ServiceResultStatus.Conflict);
            }

            // Defensive guard: this endpoint is only meaningful for an org-scoped
            // caller. A platform admin (OrganizationId == null) hitting this by
            // mistake would otherwise insert a role belonging to no organization
            // at all — caught here explicitly rather than relying on the FK
            // constraint to reject it with an opaque 500.
            if (_tenant.OrganizationId is null)
            {
                return ServiceResult<RoleResponse>.Failure(
                    "Roles must be created within an organization context.", ServiceResultStatus.BadRequest);
            }

            var role = new Role
            {
                Id = Guid.NewGuid(),
                OrganizationId = _tenant.OrganizationId.Value,
                Name = trimmedName,
                Permissions = request.Permissions.GetRawText(),
                IsSystemDefault = false // user-created roles are never flagged as defaults
            };

            _roleRepository.Add(role);
            await _roleRepository.SaveChangesAsync();

            return ServiceResult<RoleResponse>.Success(MapToResponse(role), ServiceResultStatus.Created);
        }

        public async Task<ServiceResult<RoleResponse>> UpdateAsync(Guid id, UpdateRoleRequest request)
        {
            var role = await _roleRepository.GetByIdAsync(id);
            if (role is null)
            {
                return ServiceResult<RoleResponse>.Failure("Role not found.", ServiceResultStatus.NotFound);
            }

            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return ServiceResult<RoleResponse>.Failure("Role name is required.", ServiceResultStatus.BadRequest);
            }

            var trimmedName = request.Name.Trim();

            // Only block the rename if a DIFFERENT role already owns that name —
            // renaming a role to the name it already has is a no-op, not a conflict.
            if (!string.Equals(role.Name, trimmedName, StringComparison.Ordinal)
                && await _roleRepository.NameExistsInOrganizationAsync(trimmedName))
            {
                return ServiceResult<RoleResponse>.Failure(
                    $"A role named '{trimmedName}' already exists in this organization.",
                    ServiceResultStatus.Conflict);
            }

            role.Name = trimmedName;
            role.Permissions = request.Permissions.GetRawText();
            // IsSystemDefault deliberately untouched here — editing permissions
            // on a seeded role doesn't strip its "default" badge; only deletion
            // (handled entirely separately) removes it from the org.

            await _roleRepository.SaveChangesAsync();

            return ServiceResult<RoleResponse>.Success(MapToResponse(role));
        }

        public async Task<ServiceResult<string>> DeleteAsync(Guid id)
        {
            var role = await _roleRepository.GetByIdAsync(id);
            if (role is null)
            {
                return ServiceResult<string>.Failure("Role not found.", ServiceResultStatus.NotFound);
            }

            var affectedUserCount = await _userRepository.CountByRoleIdAsync(id);

            _roleRepository.Remove(role);
            await _roleRepository.SaveChangesAsync();

            var message = affectedUserCount > 0
                ? $"Role deleted. {affectedUserCount} user(s) had this role and now have no role assigned — reassign them to restore their access."
                : "Role deleted.";

            return ServiceResult<string>.Success(message);
        }

        private RoleResponse MapToResponse(Role role)
        {
            return new RoleResponse
            {
                Id = role.Id,
                Name = role.Name,
                Permissions = JsonSerializer.Deserialize<JsonElement>(role.Permissions),
                IsSystemDefault = role.IsSystemDefault,
                AssignedUserCount = role.Users?.Count ?? 0
            };
        }
    }
}
