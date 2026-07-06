using Domain.Enum;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Sales
{
    public class CreateActivityRequest
    {
        [Required]
        public Guid LeadId { get; set; }

        [Required]
        public ActivityType Type { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Notes { get; set; } = string.Empty;

        [Required]
        public DateTime ActivityDateTime { get; set; }
    }
}
