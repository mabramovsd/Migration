using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migration.Shipbuilding.Migrations
{
    /// <inheritdoc />
    public partial class Professions : Microsoft.EntityFrameworkCore.Migrations.Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //Table
            migrationBuilder.CreateTable(
                name: "Professions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", collation: "Cyrillic_General_CI_AS", unicode: true, nullable: false),
                    Column = table.Column<string>(type: "nvarchar(max)", unicode: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Professions", x => x.Id);
                });

            //Seed (вместо raw SQL)
            migrationBuilder.InsertData(
                table: "Professions",
                columns: new[] { "Id", "Title", "Column" },
                values: new object[,]
                {
                    {
                        Guid.Parse("a1a1a1a1-a1a1-0123-abcd-aa1234567890"),
                        "Все",
                        "All"
                    },
                    {
                        Guid.Parse("a1a1a1a1-a1a1-0123-abcd-aa1234567891"),
                        "Плотник",
                        "CanCarpentry"
                    },
                    {
                        Guid.Parse("a1a1a1a1-a1a1-0123-abcd-aa1234567892"),
                        "Проектировщик",
                        "CanDesignShip"
                    },
                    {
                        Guid.Parse("a1a1a1a1-a1a1-0123-abcd-aa1234567893"),
                        "Сварщик",
                        "CanWeld"
                    }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //Seed rollback
            migrationBuilder.DeleteData(
                table: "Professions",
                keyColumn: "Id",
                keyValues: new object[]
                {
                    Guid.Parse("a1a1a1a1-a1a1-0123-abcd-aa1234567890"),
                    Guid.Parse("a1a1a1a1-a1a1-0123-abcd-aa1234567891"),
                    Guid.Parse("a1a1a1a1-a1a1-0123-abcd-aa1234567892"),
                    Guid.Parse("a1a1a1a1-a1a1-0123-abcd-aa1234567893")
                });

            //Table
            migrationBuilder.DropTable(
                name: "Professions");
        }
    }
}