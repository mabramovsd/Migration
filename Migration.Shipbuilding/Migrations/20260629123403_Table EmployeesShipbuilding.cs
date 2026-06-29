using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migration.Shipbuilding.Migrations
{
    /// <inheritdoc />
    public partial class TableEmployeesShipbuilding : Microsoft.EntityFrameworkCore.Migrations.Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmployeesShipbuilding",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CanCarpentry = table.Column<bool>(type: "bit", nullable: false),
                    CanDesignShip = table.Column<bool>(type: "bit", nullable: false),
                    CanWeld = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeesShipbuilding", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmployeesShipbuilding");
        }
    }
}
