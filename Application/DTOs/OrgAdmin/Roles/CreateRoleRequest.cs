using System.Text.Json;

namespace Application.DTOs.OrgAdmin.Roles
{
    public class CreateRoleRequest
    {
        public string Name { get; set; } = string.Empty;

        /// <summary>Arbitrary JSON object, e.g. {"leads_view":"team","leads_assign":true,...}.
        /// Accepted as a raw JsonElement so org admins aren't restricted to a
        /// fixed set of permission keys — custom roles can define whatever keys
        /// the application chooses to check later.</summary>
        public JsonElement Permissions { get; set; }
    }
}
