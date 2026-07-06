namespace Infrastructure.Contracts
{
    /// <summary>
    /// Resolved once per HTTP request (typically in middleware, from the JWT's
    /// "org_id" claim) and injected as scoped. Both the EF Core global query
    /// filters and the RLS session-variable setter read from this single source
    /// of truth, so the two enforcement layers can never disagree about which
    /// tenant is "current".
    ///
    /// Platform admins are a special case: IsPlatformAdmin is true and
    /// OrganizationId is null, meaning "no tenant filter applies" — handled
    /// explicitly in the query filter predicate, not by leaving filters off.
    /// </summary>
    public interface ICurrentTenantService
    {
        Guid? OrganizationId { get; }
        Guid? UserId { get; }
        bool IsPlatformAdmin { get; }
        bool IsOrgAdmin { get; }
    }
}
