namespace Domain.Entities
{
    /// <summary>
    /// A user belongs to exactly one organization (no cross-org membership in
    /// this MVP — "workspace switching" as a bonus feature would relax this
    /// later via a join table, but the brief's core requirement is strict
    /// per-org isolation, so we start strict).
    /// </summary>
    public class User
    {
        public Guid Id { get; set; }

        /// <summary>Null only for platform administrators, who don't belong to any
        /// tenant. Every org-scoped user must have this set.</summary>
        public Guid? OrganizationId { get; set; }
        public Organization? Organization { get; set; } = null!;

        public Guid? TeamId { get; set; }
        public Team? Team { get; set; }

        public Guid? RoleId { get; set; }
        public Role? Role { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        /// <summary>Hashed password (ASP.NET Identity or a manual hasher — decide in auth step).</summary>
        public string PasswordHash { get; set; } = string.Empty;

        /// <summary>True only for the platform-level admin who manages organizations themselves.
        /// Platform admins have OrganizationId pointing at a reserved system org, or null —
        /// decide this explicitly when we build platform admin auth.</summary>
        public bool IsPlatformAdmin { get; set; } = false;

        /// <summary>True for the org admin created during organization onboarding.</summary>
        public bool IsOrgAdmin { get; set; } = false;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Lead> AssignedLeads { get; set; } = [];

        public ICollection<Activity> CreatedActivities { get; set; } = [];

        public ICollection<LeadStatusHistory> LeadStatusChanges { get; set; } = [];
    }
}
