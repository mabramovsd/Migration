using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migration.Contracts.Migrations
{
    /// <inheritdoc />
    public partial class SeedDataForSchoolAndNursery : Microsoft.EntityFrameworkCore.Migrations.Migration
    {
        /// <inheritdoc />
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
                        Guid.Parse("10101010-1010-1010-1010-101010101010"),
                        new DateTime(1930, 11, 03),
                        "Бабка Грэнни",
                        "NurseryHome",
                        false
                    },
                    {
                        Guid.Parse("20202020-2020-2020-2020-202020202020"),
                        new DateTime(1928, 12, 05),
                        "Дед Мазай",
                        "NurseryHome",
                        false
                    },
                    {
                        Guid.Parse("01010101-0101-0101-0101-010101010101"),
                        new DateTime(2012, 05, 22),
                        "Спиногрыз 2 уровня",
                        "School",
                        false
                    },
                    {
                        Guid.Parse("02020202-0202-0202-0202-020202020202"),
                        new DateTime(2016, 12, 02),
                        "Спиногрыз",
                        "School",
                        false
                    }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValues: new object[]
                {
                    Guid.Parse("10101010-1010-1010-1010-101010101010"),
                    Guid.Parse("20202020-2020-2020-2020-202020202020"),
                    Guid.Parse("01010101-0101-0101-0101-010101010101"),
                    Guid.Parse("02020202-0202-0202-0202-020202020202")
                });
        }
    }
}
