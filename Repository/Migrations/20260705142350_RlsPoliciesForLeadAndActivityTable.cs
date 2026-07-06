using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RlsPoliciesForLeadAndActivityTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                ALTER TABLE leads                   ENABLE ROW LEVEL SECURITY;
                ALTER TABLE activities              ENABLE ROW LEVEL SECURITY;
                ALTER TABLE lead_status_histories   ENABLE ROW LEVEL SECURITY;

                ALTER TABLE leads                   FORCE ROW LEVEL SECURITY;
                ALTER TABLE activities              FORCE ROW LEVEL SECURITY;
                ALTER TABLE lead_status_histories   FORCE ROW LEVEL SECURITY;
 
                CREATE POLICY tenant_isolation_leads ON leads
                    USING (organization_id = current_setting('app.current_org_id', true)::uuid);

                CREATE POLICY tenant_isolation_activities ON activities
                    USING (organization_id = current_setting('app.current_org_id', true)::uuid);

                CREATE POLICY tenant_isolation_lead_status_histories ON lead_status_histories
                    USING (organization_id = current_setting('app.current_org_id', true)::uuid);
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DROP POLICY IF EXISTS tenant_isolation_lead_status_histories ON lead_status_histories;
                DROP POLICY IF EXISTS tenant_isolation_activities ON activities;
                DROP POLICY IF EXISTS tenant_isolation_leads ON leads;
            ");
        }
    }
}
