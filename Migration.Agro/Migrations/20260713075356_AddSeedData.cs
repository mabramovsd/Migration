using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migration.Agro.Migrations
{
    /// <inheritdoc />
    public partial class AddSeedData : Microsoft.EntityFrameworkCore.Migrations.Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("INSERT INTO EmployeesAgro (Id, HasTracktorLicense, IsDeleted) VALUES ('a1b2c3d4-e5f6-7890-abcd-ef1234567890', 1, 0)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM EmployeesAgro WHERE Id = 'a1b2c3d4-e5f6-7890-abcd-ef1234567890'");
        }
    }
}
