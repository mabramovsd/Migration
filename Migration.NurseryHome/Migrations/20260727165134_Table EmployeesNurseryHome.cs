using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migration.NurseryHome.Migrations
{
    /// <inheritdoc />
    public partial class TableEmployeesNurseryHome : Microsoft.EntityFrameworkCore.Migrations.Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmployeesNurseryHome",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeesNurseryHome", x => x.Id);
                });

            // Seed initial data for Nursery Home
            migrationBuilder.InsertData(
                table: "EmployeesNurseryHome",
                columns: new[] { "Id", "IsDeleted" },
                values: new object[,]
                {
                    { new Guid("10101010-1010-1010-1010-101010101010"), false },
                    { new Guid("20202020-2020-2020-2020-202020202020"), false }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmployeesNurseryHome");
        }
    }
}
