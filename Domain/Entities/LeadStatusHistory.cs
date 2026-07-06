using Domain.Enum;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class LeadStatusHistory : BaseEntity
    {
        [Required]
        public Guid OrganizationId { get; set; }

        public Organization Organization { get; set; } = null!;

        [Required]
        public Guid LeadId { get; set; }

        public Lead Lead { get; set; } = null!;

        public LeadStatus FromStatus { get; set; }

        public LeadStatus ToStatus { get; set; }

        [Required]
        public Guid ChangedByUserId { get; set; }

        public User ChangedByUser { get; set; } = null!;
    }
}
