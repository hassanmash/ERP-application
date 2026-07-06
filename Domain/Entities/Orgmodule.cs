namespace Domain.Entities
{
    /// <summary>
    /// Implements "enable/disable modules per organization" from the brief.
    /// Also doubles as the seed mechanism for feature flags later — same shape,
    /// just keyed by a flag name instead of a module name.
    /// </summary>
    public class OrgModule
    {
        public Guid Id { get; set; }

        public Guid OrganizationId { get; set; }
        public Organization Organization { get; set; } = null!;

        /// <summary>e.g. "SalesErp", "Notifications"</summary>
        public string ModuleName { get; set; } = string.Empty;

        public bool IsEnabled { get; set; } = true;
    }
}
