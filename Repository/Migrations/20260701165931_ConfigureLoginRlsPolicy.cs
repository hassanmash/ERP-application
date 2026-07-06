using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ConfigureLoginRlsPolicy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Create the targeted login lookup policy (Option B)
            migrationBuilder.Sql(@"
                CREATE POLICY allow_login_check ON users 
                FOR SELECT 
                USING (
                    email = current_setting('app.login_attempt_email', true)
                );
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Clean up the policies if you ever roll back the migration
            migrationBuilder.Sql(@"
                DROP POLICY IF EXISTS allow_login_check ON users;
            ");
        }
    }
}
