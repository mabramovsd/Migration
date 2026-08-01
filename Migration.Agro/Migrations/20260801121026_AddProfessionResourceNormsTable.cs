using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migration.Agro.Migrations
{
    /// <inheritdoc />
    public partial class AddProfessionResourceNormsTable : Microsoft.EntityFrameworkCore.Migrations.Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProfessionResourceNorms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProfessionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ResourceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Hours = table.Column<int>(type: "int", nullable: false),
                    QuantityProduced = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfessionResourceNorms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProfessionResourceNorms_Professions_ProfessionId",
                        column: x => x.ProfessionId,
                        principalTable: "Professions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProfessionResourceNorms_ResourcesAgro_ResourceId",
                        column: x => x.ResourceId,
                        principalTable: "ResourcesAgro",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProfessionResourceNorms_ProfessionId",
                table: "ProfessionResourceNorms",
                column: "ProfessionId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfessionResourceNorms_ResourceId",
                table: "ProfessionResourceNorms",
                column: "ResourceId");

            // Seed data: ProfessionResourceNorms
            // PoultryFarmer (a1a1a1a1-a1a1-0123-abcd-aa1234567894) → Eggs (22222222-2222-2222-2222-222222222222): 30 eggs per 1 hour
            // VegetableGrower (a1a1a1a1-a1a1-0123-abcd-aa1234567896) → Potato (11111111-1111-1111-1111-111111111111): 10 hours for 100kg
            // TractorDriver (a1a1a1a1-a1a1-0123-abcd-aa1234567891) → Potato (11111111-1111-1111-1111-111111111111): 1 hour for 100kg
            // VegetableGrower (a1a1a1a1-a1a1-0123-abcd-aa1234567896) → Potato (11111111-1111-1111-1111-111111111111): 20 hours for 100kg

            migrationBuilder.InsertData(
                table: "ProfessionResourceNorms",
                columns: new[] { "Id", "ProfessionId", "ResourceId", "Hours", "QuantityProduced" },
                values: new object[,]
                {
                    { new Guid("b1b1b1b1-b1b1-0123-abcd-aa1234567891"), Guid.Parse("a1a1a1a1-a1a1-0123-abcd-aa1234567894"), Guid.Parse("22222222-2222-2222-2222-222222222222"), 1, 30 },
                    { new Guid("b1b1b1b1-b1b1-0123-abcd-aa1234567892"), Guid.Parse("a1a1a1a1-a1a1-0123-abcd-aa1234567896"), Guid.Parse("11111111-1111-1111-1111-111111111111"), 10, 100 },
                    { new Guid("b1b1b1b1-b1b1-0123-abcd-aa1234567893"), Guid.Parse("a1a1a1a1-a1a1-0123-abcd-aa1234567891"), Guid.Parse("11111111-1111-1111-1111-111111111111"), 1, 100 },
                    { new Guid("b1b1b1b1-b1b1-0123-abcd-aa1234567894"), Guid.Parse("a1a1a1a1-a1a1-0123-abcd-aa1234567896"), Guid.Parse("11111111-1111-1111-1111-111111111111"), 20, 100 },
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProfessionResourceNorms");
        }
    }
}
