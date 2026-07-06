namespace Domain.Entities
{
    public abstract class OrganizationEntity : BaseEntity
    {
        public Guid OrganizationId { get; set; }

        public Organization Organization { get; set; } = null!;
    }
}
