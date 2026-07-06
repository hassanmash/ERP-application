using Domain.Enum;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Lead : BaseEntity
    {
        public Guid OrganizationId { get; set; }
        public Organization Organization { get; set; } = null!;
        public string Name { get; set; } = "";
        public string MobileNumber { get; set; } = "";
        public string? Email { get; set; }
        public string Source { get; set; } = "";
        public string Project { get; set; } = "";
        public LeadStatus Status { get; set; } = LeadStatus.New;
        public Guid AssignedUserId { get; set; }
        public User AssignedUser { get; set; } = null!;
        public ICollection<Activity> Activities { get; set; } = [];
        public ICollection<LeadStatusHistory> StatusHistory { get; set; } = [];
    }
}
