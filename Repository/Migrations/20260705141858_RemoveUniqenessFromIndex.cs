using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUniqenessFromIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_leads_assigned_user_id",
                table: "leads");

            migrationBuilder.DropIndex(
                name: "ix_leads_organization_id",
                table: "leads");

            migrationBuilder.DropIndex(
                name: "ix_leads_status",
                table: "leads");

            migrationBuilder.DropIndex(
                name: "ix_lead_status_histories_changed_by_user_id",
                table: "lead_status_histories");

            migrationBuilder.DropIndex(
                name: "ix_lead_status_histories_lead_id",
                table: "lead_status_histories");

            migrationBuilder.DropIndex(
                name: "ix_lead_status_histories_organization_id",
                table: "lead_status_histories");

            migrationBuilder.DropIndex(
                name: "ix_activities_activity_date_time",
                table: "activities");

            migrationBuilder.DropIndex(
                name: "ix_activities_created_by_user_id",
                table: "activities");

            migrationBuilder.DropIndex(
                name: "ix_activities_lead_id",
                table: "activities");

            migrationBuilder.DropIndex(
                name: "ix_activities_organization_id",
                table: "activities");

            migrationBuilder.CreateIndex(
                name: "ix_leads_assigned_user_id",
                table: "leads",
                column: "assigned_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_leads_organization_id",
                table: "leads",
                column: "organization_id");

            migrationBuilder.CreateIndex(
                name: "ix_leads_status",
                table: "leads",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ix_lead_status_histories_changed_by_user_id",
                table: "lead_status_histories",
                column: "changed_by_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_lead_status_histories_lead_id",
                table: "lead_status_histories",
                column: "lead_id");

            migrationBuilder.CreateIndex(
                name: "ix_lead_status_histories_organization_id",
                table: "lead_status_histories",
                column: "organization_id");

            migrationBuilder.CreateIndex(
                name: "ix_activities_created_by_user_id",
                table: "activities",
                column: "created_by_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_activities_lead_id",
                table: "activities",
                column: "lead_id");

            migrationBuilder.CreateIndex(
                name: "ix_activities_organization_id",
                table: "activities",
                column: "organization_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_leads_assigned_user_id",
                table: "leads");

            migrationBuilder.DropIndex(
                name: "ix_leads_organization_id",
                table: "leads");

            migrationBuilder.DropIndex(
                name: "ix_leads_status",
                table: "leads");

            migrationBuilder.DropIndex(
                name: "ix_lead_status_histories_changed_by_user_id",
                table: "lead_status_histories");

            migrationBuilder.DropIndex(
                name: "ix_lead_status_histories_lead_id",
                table: "lead_status_histories");

            migrationBuilder.DropIndex(
                name: "ix_lead_status_histories_organization_id",
                table: "lead_status_histories");

            migrationBuilder.DropIndex(
                name: "ix_activities_created_by_user_id",
                table: "activities");

            migrationBuilder.DropIndex(
                name: "ix_activities_lead_id",
                table: "activities");

            migrationBuilder.DropIndex(
                name: "ix_activities_organization_id",
                table: "activities");

            migrationBuilder.CreateIndex(
                name: "ix_leads_assigned_user_id",
                table: "leads",
                column: "assigned_user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_leads_organization_id",
                table: "leads",
                column: "organization_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_leads_status",
                table: "leads",
                column: "status",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_lead_status_histories_changed_by_user_id",
                table: "lead_status_histories",
                column: "changed_by_user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_lead_status_histories_lead_id",
                table: "lead_status_histories",
                column: "lead_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_lead_status_histories_organization_id",
                table: "lead_status_histories",
                column: "organization_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_activities_activity_date_time",
                table: "activities",
                column: "activity_date_time",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_activities_created_by_user_id",
                table: "activities",
                column: "created_by_user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_activities_lead_id",
                table: "activities",
                column: "lead_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_activities_organization_id",
                table: "activities",
                column: "organization_id",
                unique: true);
        }
    }
}
