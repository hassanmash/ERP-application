namespace Application.DTOs.PlatformAdmin
{
    public class CreateOrganizationRequest
    {
        public required string Name { get; set; }
        public required string OrgCode { get; set; }
        public string? LogoUrl { get; set; }
        public string? Description { get; set; }

        /// <summary>Module names to enable at creation time, e.g. ["SalesErp", "Notifications"].
        /// Defaults to none if omitted — modules are opt-in per the brief, not opt-out.</summary>
        public List<string> EnabledModules { get; set; } = [];

        public required string AdminName { get; set; }
        public required string AdminEmail { get; set; }
        public required string AdminPassword { get; set; }
    }
}
