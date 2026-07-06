namespace Domain.Entities
{
    public enum OrganizationStatus
    {
        Active,
        Suspended
    }

    /// <summary>
    /// The tenant root. Every other tenant-scoped entity carries an OrganizationId
    /// that must reference a row here. Nothing about onboarding a new org touches
    /// code or deployments — it's a single insert into this table (plus an admin user).
    /// </summary>
    public class Organization
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        /// <summary>Short unique code, e.g. "HILITE-BLD". Used in URLs/subdomains later if needed.</summary>
        public string OrgCode { get; set; } = string.Empty;

        public string? LogoUrl { get; set; }

        public string? Description { get; set; }

        public OrganizationStatus Status { get; set; } = OrganizationStatus.Active;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation
        public ICollection<User> Users { get; set; } = new List<User>();
        public ICollection<Team> Teams { get; set; } = new List<Team>();
        public ICollection<Role> Roles { get; set; } = new List<Role>();
        public ICollection<OrgModule> OrgModules { get; set; } = new List<OrgModule>();
    }
}
