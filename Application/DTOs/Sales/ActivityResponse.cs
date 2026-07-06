using Domain.Enum;

namespace Application.DTOs.Sales
{
    public class ActivityResponse
    {
        public Guid Id { get; set; }

        public Guid LeadId { get; set; }

        public ActivityType Type { get; set; }

        public string Notes { get; set; } = string.Empty;

        public DateTime ActivityDateTime { get; set; }

        public Guid CreatedByUserId { get; set; }

        public string CreatedByUserName { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }
}
