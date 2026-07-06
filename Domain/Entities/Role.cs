namespace Domain.Entities
{
    /// <summary>
    /// Org-scoped, custom-definable role. Permissions are stored as JSON rather
    /// than a fixed enum so org admins can create roles beyond the suggested
    /// defaults (Executive, Team Lead, Sales Manager, Director) without a code
    /// change or migration. Mapped to jsonb in Postgres.
    ///
    /// Example Permissions value:
    /// {
    ///   "leads.view": "team",      // "own" | "team" | "org"
    ///   "leads.assign": true,
    ///   "dashboard.view": "team",
    ///   "users.manage": false
    /// }
    /// </summary>
    public class Role
    {
        public Guid Id { get; set; }

        public Guid OrganizationId { get; set; }
        public Organization Organization { get; set; } = null!;

        public string Name { get; set; } = string.Empty;

        /// <summary>JSON-serialized permission map. Stored as jsonb column.</summary>
        public string Permissions { get; set; } = "{}";

        public bool IsSystemDefault { get; set; } = false;

        // Navigation
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
