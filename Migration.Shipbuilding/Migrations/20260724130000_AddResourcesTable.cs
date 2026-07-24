using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migration.Shipbuilding.Migrations
{
    /// <inheritdoc />
    public partial class AddResourcesTable : Microsoft.EntityFrameworkCore.Migrations.Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ResourcesShipbuilding",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Count = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResourcesShipbuilding", x => x.Id);
                });

            // Seed initial data for ResourcesShipbuilding
            migrationBuilder.InsertData(
                table: "ResourcesShipbuilding",
                columns: new[] { "Id", "Title", "Count", "Unit" },
                values: new object[,]
                {
                    { new Guid("55555555-5555-5555-5555-555555555555"), "Весельная лодка", 2m, "шт" },
                    { new Guid("66666666-6666-6666-6666-666666666666"), "Корабль дальнего плавания", 1m, "шт" },
                    { new Guid("77777777-7777-7777-7777-777777777777"), "Плот", 5m, "шт" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ResourcesShipbuilding");
        }
    }
}
