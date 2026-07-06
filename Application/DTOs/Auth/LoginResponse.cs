namespace Application.DTOs.Auth
{
    /// <summary>
    /// Single response shape for both user types — OrganizationId/OrganizationName
    /// are simply null for platform admins. The Angular app branches on
    /// IsPlatformAdmin to decide which app shell/routes to load.
    /// </summary>
    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsPlatformAdmin { get; set; }
        public bool IsOrgAdmin { get; set; }
        public Guid? OrganizationId { get; set; }
        public string? OrganizationName { get; set; }
    }
}
