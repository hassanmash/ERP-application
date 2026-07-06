using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Sales
{
    public class UpdateLeadRequest
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string MobileNumber { get; set; } = string.Empty;

        [EmailAddress]
        [MaxLength(200)]
        public string? Email { get; set; }

        [Required]
        [MaxLength(100)]
        public string Source { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string Project { get; set; } = string.Empty;

        [Required]
        public Guid AssignedUserId { get; set; }
    }
}
