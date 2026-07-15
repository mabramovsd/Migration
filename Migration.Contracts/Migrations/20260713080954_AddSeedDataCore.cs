using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migration.Contracts.Migrations
{
    /// <inheritdoc />
    public partial class AddSeedDataCore : Microsoft.EntityFrameworkCore.Migrations.Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Insert seed data for Employees table in a single batch operation.
            // Using object[,] array where each inner array represents one row.
            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "BirthDate", "FullName", "CurrentCompany", "IsDeleted" },
                values: new object[,]
                {
                    {
                        Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567890"),
                        new DateTime(1990, 5, 15),
                        "Иван Иванов",
                        "Agro",
                        false
                    },
                    {
                        Guid.Parse("b2c3d4e5-f6a7-8901-bcde-f12345678901"),
                        new DateTime(1996, 12, 15),
                        "Пётр Петров",
                        "Second Company",
                        false
                    },
                    {
                        Guid.Parse("c1b2c3d4-e5f6-7890-abcd-ef1234567890"),
                        new DateTime(1989, 8, 6),
                        "Семён Семёнов",
                        "Shipbuilding",
                        false
                    }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove all seeded employee records by their primary keys (Ids).
            // This executes before dropping any dependent tables if they existed.
            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValues: new object[]
                {
                    Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567890"),
                    Guid.Parse("b2c3d4e5-f6a7-8901-bcde-f12345678901"),
                    Guid.Parse("c1b2c3d4-e5f6-7890-abcd-ef1234567890")
                });
        }
    }
}