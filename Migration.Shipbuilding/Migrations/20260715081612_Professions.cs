using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migration.Shipbuilding.Migrations
{
    /// <inheritdoc />
    public partial class Professions : Microsoft.EntityFrameworkCore.Migrations.Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //Table
            migrationBuilder.CreateTable(
                name: "Professions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Column = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Professions", x => x.Id);
                });

            //Seed
            migrationBuilder.Sql($@"
                INSERT INTO Professions (Id, Title, [Column])
                VALUES (
                    'a1a1a1a1-a1a1-0123-abcd-aa1234567890', 
                    'Все', 
                    'All'
                ),
                (
                    'a1a1a1a1-a1a1-0123-abcd-aa1234567891', 
                    'Плотник', 
                    'CanCarpentry'
                ),
                (
                    'a1a1a1a1-a1a1-0123-abcd-aa1234567892', 
                    'Проектировщик', 
                    'CanDesignShip'
                ),
                (
                    'a1a1a1a1-a1a1-0123-abcd-aa1234567893', 
                    'Сварщик', 
                    'CanWeld'
                );"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //Seed
            migrationBuilder.Sql($@"
                DELETE FROM Professions
                WHERE Id IN (
                    'a1a1a1a1-a1a1-0123-abcd-aa1234567890',
                    'a1a1a1a1-a1a1-0123-abcd-aa1234567891',
                    'a1a1a1a1-a1a1-0123-abcd-aa1234567892',
                    'a1a1a1a1-a1a1-0123-abcd-aa1234567893'
                )"
            );
            //Table
            migrationBuilder.DropTable(
                name: "Professions");
        }
    }
}
