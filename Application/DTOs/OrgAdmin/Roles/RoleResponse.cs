using System.Text.Json;

namespace Application.DTOs.OrgAdmin.Roles
{
    public class RoleResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public JsonElement Permissions { get; set; }
        public bool IsSystemDefault { get; set; }
        public int AssignedUserCount { get; set; }
    }
}
