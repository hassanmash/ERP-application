using Domain.Enum;

namespace Application.Services.Security
{
    public interface IPermissionService
    {
        Task<DataScope> GetLeadViewScopeAsync();

        Task<DataScope> GetDashboardViewScopeAsync();

        Task<bool> CanAssignLeadsAsync();

        Task<bool> CanManageUsersAsync();
    }
}
