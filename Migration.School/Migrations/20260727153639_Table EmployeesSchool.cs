using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migration.School.Migrations
{
    /// <inheritdoc />
    public partial class TableEmployeesSchool : Microsoft.EntityFrameworkCore.Migrations.Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmployeesSchool",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeesSchool", x => x.Id);
                });


            // Seed initial data for ResourcesShipbuilding
            migrationBuilder.InsertData(
                table: "EmployeesSchool",
                columns: new[] { "Id", "IsDeleted" },
                values: new object[,]
                {
                    { new Guid("01010101-0101-0101-0101-010101010101"), false },
                    { new Guid("02020202-0202-0202-0202-020202020202"), false }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmployeesSchool");
        }
    }
}
