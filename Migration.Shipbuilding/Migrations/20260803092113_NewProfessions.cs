using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migration.Shipbuilding.Migrations
{
    /// <inheritdoc />
    public partial class NewProfessions : Microsoft.EntityFrameworkCore.Migrations.Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CanPaint",
                table: "EmployeesShipbuilding",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CanRig",
                table: "EmployeesShipbuilding",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CanShipyard",
                table: "EmployeesShipbuilding",
                type: "bit",
                nullable: false,
                defaultValue: false);

            //Seed
            migrationBuilder.InsertData(
                table: "Professions",
                columns: new[] { "Id", "Title", "Column" },
                values: new object[,]
                {
                    {
                        Guid.Parse("a1a1a1a1-a1a1-0123-abcd-aa1234567894"),
                        "Рабочий верфи",
                        "CanShipyard"
                    },
                    {
                        Guid.Parse("a1a1a1a1-a1a1-0123-abcd-aa1234567895"),
                        "Красильщик",
                        "CanPaint"
                    },
                    {
                        Guid.Parse("a1a1a1a1-a1a1-0123-abcd-aa1234567896"),
                        "Такелажник",
                        "CanRig"
                    }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ResourcesShipbuilding");

            migrationBuilder.DropColumn(
                name: "CanPaint",
                table: "EmployeesShipbuilding");

            migrationBuilder.DropColumn(
                name: "CanRig",
                table: "EmployeesShipbuilding");

            migrationBuilder.DropColumn(
                name: "CanShipyard",
                table: "EmployeesShipbuilding");

            //Seed rollback
            migrationBuilder.DeleteData(
                table: "Professions",
                keyColumn: "Id",
                keyValues: new object[]
                {
                    Guid.Parse("a1a1a1a1-a1a1-0123-abcd-aa1234567894"),
                    Guid.Parse("a1a1a1a1-a1a1-0123-abcd-aa1234567895"),
                    Guid.Parse("a1a1a1a1-a1a1-0123-abcd-aa1234567896")
                });
        }
    }
}
