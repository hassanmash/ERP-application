using Domain.Enum;

namespace Application.DTOs.Sales
{
    public class LeadResponse
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string MobileNumber { get; set; } = string.Empty;

        public string? Email { get; set; }

        public string Source { get; set; } = string.Empty;

        public string Project { get; set; } = string.Empty;

        public LeadStatus Status { get; set; }

        public Guid AssignedUserId { get; set; }

        public string AssignedUserName { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }
}
