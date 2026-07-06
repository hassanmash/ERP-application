using Application.Constants;
using Domain.Enum;
using Infrastructure.Contracts;
using Infrastructure.Repositories.Contracts;
using System.Text.Json;

namespace Application.Services.Security
{
    public class PermissionService(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        ICurrentTenantService tenant) : IPermissionService, IDisposable
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IRoleRepository _roleRepository = roleRepository;
        private readonly ICurrentTenantService _tenant = tenant;

        private JsonDocument? _permissionDocument;
        private bool _permissionsLoaded;

        public Task<DataScope> GetLeadViewScopeAsync()
            => GetScopePermissionAsync(
                PermissionKeys.LeadsView,
                DataScope.Own);

        public Task<DataScope> GetDashboardViewScopeAsync()
            => GetScopePermissionAsync(
                PermissionKeys.DashboardView,
                DataScope.Own);

        public Task<bool> CanAssignLeadsAsync()
            => GetBooleanPermissionAsync(
                PermissionKeys.LeadsAssign);

        public Task<bool> CanManageUsersAsync()
            => GetBooleanPermissionAsync(
                PermissionKeys.UsersManage);

        public void Dispose()
        {
            _permissionDocument?.Dispose();
            GC.SuppressFinalize(this);
        }

        private async Task<bool> GetBooleanPermissionAsync(
            string key,
            bool defaultValue = false)
        {
            if (_tenant.IsPlatformAdmin)
                return true;

            if (_tenant.IsOrgAdmin)
                return true;

            var permissions = await GetPermissionsAsync();

            if (permissions == null)
                return defaultValue;

            if (!permissions.Value.TryGetProperty(key, out var value))
                return defaultValue;

            if (value.ValueKind != JsonValueKind.True &&
                value.ValueKind != JsonValueKind.False)
            {
                return defaultValue;
            }

            return value.GetBoolean();
        }

        private async Task<DataScope> GetScopePermissionAsync(
            string key,
            DataScope defaultScope = DataScope.Own)
        {
            if (_tenant.IsPlatformAdmin)
                return DataScope.Organization;

            if (_tenant.IsOrgAdmin)
                return DataScope.Organization;

            var permissions = await GetPermissionsAsync();

            if (permissions == null)
                return defaultScope;

            if (!permissions.Value.TryGetProperty(key, out var value))
                return defaultScope;

            if (value.ValueKind != JsonValueKind.String)
                return defaultScope;

            return value.GetString()?.Trim().ToLowerInvariant() switch
            {
                "organization" => DataScope.Organization,
                "team" => DataScope.Team,
                "own" => DataScope.Own,
                _ => defaultScope
            };
        }

        private async Task<JsonElement?> GetPermissionsAsync()
        {
            if (_permissionsLoaded)
                return _permissionDocument?.RootElement;

            _permissionsLoaded = true;

            if (!_tenant.UserId.HasValue)
                return null;

            var user = await _userRepository.GetByIdAsync(_tenant.UserId.Value);

            if (user?.RoleId == null)
                return null;

            var role = await _roleRepository.GetByIdAsync(user.RoleId.Value);

            if (role == null)
                return null;

            if (string.IsNullOrWhiteSpace(role.Permissions))
                return null;

            try
            {
                _permissionDocument = JsonDocument.Parse(role.Permissions);

                return _permissionDocument.RootElement;
            }
            catch (JsonException)
            {
                return null;
            }
        }
    }
}
