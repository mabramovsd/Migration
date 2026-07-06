using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migration.Shipbuilding.Migrations
{
    /// <inheritdoc />
    public partial class IsDeletedcheckmark : Microsoft.EntityFrameworkCore.Migrations.Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "EmployeesShipbuilding",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "EmployeesShipbuilding");
        }
    }
}
