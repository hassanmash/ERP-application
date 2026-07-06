using Infrastructure.Contracts;

namespace Infrastructure.Persistence
{
    public class CurrentTenantService : ICurrentTenantService
    {
        public Guid? OrganizationId { get; private set; }
        public Guid? UserId { get; private set; }
        public bool IsPlatformAdmin { get; private set; }
        public bool IsOrgAdmin { get; private set; }

        public void SetTenant(Guid? organizationId, bool isPlatformAdmin, Guid? userId, bool isOrgAdmin)
        {
            OrganizationId = organizationId;
            IsPlatformAdmin = isPlatformAdmin;
            UserId = userId;
            IsOrgAdmin = isOrgAdmin;
        }
    }
}
