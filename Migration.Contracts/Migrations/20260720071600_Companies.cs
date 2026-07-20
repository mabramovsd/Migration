using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migration.Contracts.Migrations
{
    /// <inheritdoc />
    public partial class Companies : Microsoft.EntityFrameworkCore.Migrations.Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Latitude = table.Column<double>(type: "float", nullable: true),
                    Longitude = table.Column<double>(type: "float", nullable: true),
                    Image = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Companies",
                columns: new[] { "Id", "Name", "Latitude", "Longitude", "Image" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), "Ферма", 36.2, 24.3, "Farm.png" },
                    { new Guid("22222222-2222-2222-2222-222222222222"), "Судостроительный завод", 63.5, 76.5, "Factory.png" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Companies");
        }
    }
}
