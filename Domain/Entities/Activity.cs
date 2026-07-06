using Domain.Enum;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Activity : BaseEntity
    {
        public Guid OrganizationId { get; set; }
        public Organization Organization { get; set; } = null!;
        public Guid LeadId { get; set; }
        public Lead Lead { get; set; } = null!;
        public ActivityType Type { get; set; }
        public string Notes { get; set; } = string.Empty;
        public DateTime ActivityDateTime { get; set; }
        public Guid CreatedByUserId { get; set; }
        public User CreatedByUser { get; set; } = null!;
    }
}
