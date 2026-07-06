namespace Application.DTOs.PlatformAdmin
{
    public class UpdateOrganizationStatusRequest
    {
        /// <summary>Expected values: "Active" or "Suspended" (matches OrganizationStatus enum names).</summary>
        public string Status { get; set; } = string.Empty;
    }
}
