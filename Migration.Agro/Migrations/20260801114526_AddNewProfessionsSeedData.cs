using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migration.Agro.Migrations
{
    /// <inheritdoc />
    public partial class AddNewProfessionsSeedData : Microsoft.EntityFrameworkCore.Migrations.Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // New professions IDs
            var milkerId = Guid.Parse("a1a1a1a1-a1a1-0123-abcd-aa1234567892");
            var cattlemanId = Guid.Parse("a1a1a1a1-a1a1-0123-abcd-aa1234567893");
            var poultryFarmerId = Guid.Parse("a1a1a1a1-a1a1-0123-abcd-aa1234567894");
            var millerId = Guid.Parse("a1a1a1a1-a1a1-0123-abcd-aa1234567895");
            var vegetableGrowerId = Guid.Parse("a1a1a1a1-a1a1-0123-abcd-aa1234567896");

            migrationBuilder.InsertData(
                table: "Professions",
                columns: new[] { "Id", "Title", "Column" },
                values: new object[,]
                {
                    { milkerId, "Доярка", "IsMilker" },
                    { cattlemanId, "Скотник", "IsCattleman" },
                    { poultryFarmerId, "Птичник", "IsPoultryFarmer" },
                    { millerId, "Мельник", "IsMiller" },
                    { vegetableGrowerId, "Овощевод", "IsVegetableGrower" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            var milkerId = Guid.Parse("a1a1a1a1-a1a1-0123-abcd-aa1234567892");
            var cattlemanId = Guid.Parse("a1a1a1a1-a1a1-0123-abcd-aa1234567893");
            var poultryFarmerId = Guid.Parse("a1a1a1a1-a1a1-0123-abcd-aa1234567894");
            var millerId = Guid.Parse("a1a1a1a1-a1a1-0123-abcd-aa1234567895");
            var vegetableGrowerId = Guid.Parse("a1a1a1a1-a1a1-0123-abcd-aa1234567896");

            migrationBuilder.DeleteData(
                table: "Professions",
                keyColumn: "Id",
                keyValues: new object[] { milkerId });

            migrationBuilder.DeleteData(
                table: "Professions",
                keyColumn: "Id",
                keyValues: new object[] { cattlemanId });

            migrationBuilder.DeleteData(
                table: "Professions",
                keyColumn: "Id",
                keyValues: new object[] { poultryFarmerId });

            migrationBuilder.DeleteData(
                table: "Professions",
                keyColumn: "Id",
                keyValues: new object[] { millerId });

            migrationBuilder.DeleteData(
                table: "Professions",
                keyColumn: "Id",
                keyValues: new object[] { vegetableGrowerId });
        }
    }
}
