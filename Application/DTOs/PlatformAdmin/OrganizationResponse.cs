namespace Application.DTOs.PlatformAdmin
{
    public class OrganizationResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string OrgCode { get; set; } = string.Empty;
        public string? LogoUrl { get; set; }
        public string? Description { get; set; }
        public string Status { get; set; } = string.Empty;
        public List<string> EnabledModules { get; set; } = new();
        public Guid AdminUserId { get; set; }
        public string AdminEmail { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
