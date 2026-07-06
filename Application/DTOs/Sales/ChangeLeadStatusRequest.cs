using Domain.Enum;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Sales
{
    public class ChangeLeadStatusRequest
    {
        [Required]
        public LeadStatus Status { get; set; }
    }
}
