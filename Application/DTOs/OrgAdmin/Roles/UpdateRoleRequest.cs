using System.Text.Json;

namespace Application.DTOs.OrgAdmin.Roles
{
    public class UpdateRoleRequest
    {
        public string Name { get; set; } = string.Empty;
        public JsonElement Permissions { get; set; }
    }
}
