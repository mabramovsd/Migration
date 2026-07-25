using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migration.Contracts.Migrations
{
    /// <inheritdoc />
    public partial class AddNewCompanies : Microsoft.EntityFrameworkCore.Migrations.Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Companies",
                columns: new[] { "Id", "Name", "Alias", "Latitude", "Longitude", "Image" },
                values: new object[,]
                {
                    { new Guid("33333333-3333-3333-3333-333333333333"), "Школа", "School", 44.2, 48.3, "School.png" },
                    { new Guid("44444444-4444-4444-4444-444444444444"), "Дом престарелых", "NurseryHome", 23.5, 56.5, "NurseryHome.png" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"));

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"));
        }
    }
}
