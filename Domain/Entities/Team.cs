namespace Domain.Entities
{
    /// <summary>
    /// Org-scoped team, e.g. "North Kerala Sales". Every team belongs to exactly
    /// one organization — no cross-org teams, by design.
    /// </summary>
    public class Team
    {
        public Guid Id { get; set; }

        public Guid OrganizationId { get; set; }
        public Organization Organization { get; set; } = null!;

        public string Name { get; set; } = string.Empty;

        /// <summary>Optional — the user designated as this team's lead.</summary>
        public Guid? LeadUserId { get; set; }
        public User? LeadUser { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public ICollection<User> Members { get; set; } = new List<User>();
    }
}
