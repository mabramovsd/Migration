using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migration.Agro.Migrations
{
    /// <inheritdoc />
    public partial class AddNewProfessionsColumns : Microsoft.EntityFrameworkCore.Migrations.Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCattleman",
                table: "EmployeesAgro",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsMilker",
                table: "EmployeesAgro",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsMiller",
                table: "EmployeesAgro",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPoultryFarmer",
                table: "EmployeesAgro",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsVegetableGrower",
                table: "EmployeesAgro",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCattleman",
                table: "EmployeesAgro");

            migrationBuilder.DropColumn(
                name: "IsMilker",
                table: "EmployeesAgro");

            migrationBuilder.DropColumn(
                name: "IsMiller",
                table: "EmployeesAgro");

            migrationBuilder.DropColumn(
                name: "IsPoultryFarmer",
                table: "EmployeesAgro");

            migrationBuilder.DropColumn(
                name: "IsVegetableGrower",
                table: "EmployeesAgro");
        }
    }
}
