using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migration.Agro.Migrations
{
    /// <inheritdoc />
    public partial class AddResourcesTable : Microsoft.EntityFrameworkCore.Migrations.Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ResourcesAgro",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Count = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResourcesAgro", x => x.Id);
                });

            // Seed initial data for ResourcesAgro
            migrationBuilder.InsertData(
                table: "ResourcesAgro",
                columns: new[] { "Id", "Title", "Count", "Unit" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), "Картофель", 200m, "кг" },
                    { new Guid("22222222-2222-2222-2222-222222222222"), "Яйца", 600m, "шт" },
                    { new Guid("33333333-3333-3333-3333-333333333333"), "Мука", 100m, "кг" },
                    { new Guid("44444444-4444-4444-4444-444444444444"), "Морковь", 30m, "кг" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ResourcesAgro");
        }
    }
}
