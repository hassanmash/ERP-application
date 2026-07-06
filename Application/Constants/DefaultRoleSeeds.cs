using System.Text.Json;

namespace Application.Constants
{
    /// <summary>
    /// Seeded once per new organization at creation time. These are starting
    /// points, not fixed system roles — the org admin can edit permissions on
    /// any of these (including deleting them outright) via the Roles endpoints.
    /// IsSystemDefault on the Role entity is purely informational (lets a UI
    /// show a "default" badge); it does not gate any delete/edit logic.
    ///
    /// Permission scope values used throughout: "own" | "team" | "org".
    /// Team Lead and Sales Manager are seeded identically — the brief's worked
    /// examples (Executive/Team Lead/Director) don't distinguish Sales Manager's
    /// scope from Team Lead's, so rather than inventing an arbitrary distinction,
    /// we seed them the same and leave differentiation to the org admin if they
    /// want one.
    /// </summary>
    public static class DefaultRoleSeeds
    {
        public static readonly IReadOnlyList<(string Name, object Permissions)> Roles = new List<(string, object)>
        {
            ("Executive", new
                {
                    leads_view = "own",
                    leads_assign = false,
                    dashboard_view = "own",
                    users_manage = false
                }),
            ("Team Lead", new
                {
                    leads_view = "team",
                    leads_assign = true,
                    dashboard_view = "team",
                    users_manage = false
                }),
            ("Sales Manager", new
                {
                    leads_view = "team",
                    leads_assign = true,
                    dashboard_view = "team",
                    users_manage = false
                }),
            ("Director", new
                {
                    leads_view = "org",
                    leads_assign = true,
                    dashboard_view = "org",
                    users_manage = true
                })
        };

        public static string SerializePermissions(object permissions)
            => JsonSerializer.Serialize(permissions);
    }
}
