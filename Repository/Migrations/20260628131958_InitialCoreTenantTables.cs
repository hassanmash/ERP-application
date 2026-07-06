using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCoreTenantTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "organizations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    org_code = table.Column<string>(type: "text", nullable: false),
                    logo_url = table.Column<string>(type: "text", nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_organizations", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "org_modules",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    module_name = table.Column<string>(type: "text", nullable: false),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_org_modules", x => x.id);
                    table.ForeignKey(
                        name: "fk_org_modules_organizations_organization_id",
                        column: x => x.organization_id,
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    permissions = table.Column<string>(type: "jsonb", nullable: false),
                    is_system_default = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_roles", x => x.id);
                    table.ForeignKey(
                        name: "fk_roles_organizations_organization_id",
                        column: x => x.organization_id,
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "teams",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    lead_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_teams", x => x.id);
                    table.ForeignKey(
                        name: "fk_teams_organizations_organization_id",
                        column: x => x.organization_id,
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    team_id = table.Column<Guid>(type: "uuid", nullable: true),
                    role_id = table.Column<Guid>(type: "uuid", nullable: true),
                    name = table.Column<string>(type: "text", nullable: false),
                    email = table.Column<string>(type: "text", nullable: false),
                    password_hash = table.Column<string>(type: "text", nullable: false),
                    is_platform_admin = table.Column<bool>(type: "boolean", nullable: false),
                    is_org_admin = table.Column<bool>(type: "boolean", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                    table.ForeignKey(
                        name: "fk_users_organizations_organization_id",
                        column: x => x.organization_id,
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_users_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_users_teams_team_id",
                        column: x => x.team_id,
                        principalTable: "teams",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "ix_org_modules_organization_id_module_name",
                table: "org_modules",
                columns: new[] { "organization_id", "module_name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_organizations_org_code",
                table: "organizations",
                column: "org_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_roles_organization_id_name",
                table: "roles",
                columns: new[] { "organization_id", "name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_teams_lead_user_id",
                table: "teams",
                column: "lead_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_teams_organization_id",
                table: "teams",
                column: "organization_id");

            migrationBuilder.CreateIndex(
                name: "ix_users_organization_id_email",
                table: "users",
                columns: new[] { "organization_id", "email" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_users_role_id",
                table: "users",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "ix_users_team_id",
                table: "users",
                column: "team_id");

            migrationBuilder.AddForeignKey(
                name: "fk_teams_users_lead_user_id",
                table: "teams",
                column: "lead_user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            // =====================================================
            // Row-Level Security — raw SQL, not representable by the
            // EF Core migration builder API.
            // =====================================================
            migrationBuilder.Sql(@"
                ALTER TABLE org_modules ENABLE ROW LEVEL SECURITY;
                ALTER TABLE teams        ENABLE ROW LEVEL SECURITY;
                ALTER TABLE roles        ENABLE ROW LEVEL SECURITY;
                ALTER TABLE users        ENABLE ROW LEVEL SECURITY;
 
                ALTER TABLE org_modules FORCE ROW LEVEL SECURITY;
                ALTER TABLE teams        FORCE ROW LEVEL SECURITY;
                ALTER TABLE roles        FORCE ROW LEVEL SECURITY;
                ALTER TABLE users        FORCE ROW LEVEL SECURITY;
 
                CREATE POLICY tenant_isolation_org_modules ON org_modules
                    USING (organization_id = current_setting('app.current_org_id', true)::uuid);
 
                CREATE POLICY tenant_isolation_teams ON teams
                    USING (organization_id = current_setting('app.current_org_id', true)::uuid);
 
                CREATE POLICY tenant_isolation_roles ON roles
                    USING (organization_id = current_setting('app.current_org_id', true)::uuid);
 
                CREATE POLICY tenant_isolation_users ON users
                    USING (organization_id = current_setting('app.current_org_id', true)::uuid);
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DROP POLICY IF EXISTS tenant_isolation_users ON users;
                DROP POLICY IF EXISTS tenant_isolation_roles ON roles;
                DROP POLICY IF EXISTS tenant_isolation_teams ON teams;
                DROP POLICY IF EXISTS tenant_isolation_org_modules ON org_modules;
            ");

            migrationBuilder.DropForeignKey(
                name: "fk_roles_organizations_organization_id",
                table: "roles");

            migrationBuilder.DropForeignKey(
                name: "fk_teams_organizations_organization_id",
                table: "teams");

            migrationBuilder.DropForeignKey(
                name: "fk_users_organizations_organization_id",
                table: "users");

            migrationBuilder.DropForeignKey(
                name: "fk_teams_users_lead_user_id",
                table: "teams");

            migrationBuilder.DropTable(
                name: "org_modules");

            migrationBuilder.DropTable(
                name: "organizations");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "roles");

            migrationBuilder.DropTable(
                name: "teams");
        }
    }
}
