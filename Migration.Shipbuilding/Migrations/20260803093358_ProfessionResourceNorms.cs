using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migration.Shipbuilding.Migrations
{
    /// <inheritdoc />
    public partial class ProfessionResourceNorms : Microsoft.EntityFrameworkCore.Migrations.Migration
    {
        // Ship ids
        private static readonly Guid ResourceRowingBoat = new("55555555-5555-5555-5555-555555555555");
        private static readonly Guid ResourceLongRangeShip = new("66666666-6666-6666-6666-666666666666");
        private static readonly Guid ResourceRaft = new("77777777-7777-7777-7777-777777777777");

        // Profession ids
        private static readonly Guid ProfCarpenter = new("A1A1A1A1-A1A1-0123-ABCD-AA1234567891");
        private static readonly Guid ProfDesigner = new("A1A1A1A1-A1A1-0123-ABCD-AA1234567892");
        private static readonly Guid ProfWelder = new("A1A1A1A1-A1A1-0123-ABCD-AA1234567893");
        private static readonly Guid ProfShipyardWorker = new("A1A1A1A1-A1A1-0123-ABCD-AA1234567894");
        private static readonly Guid ProfPainter = new("A1A1A1A1-A1A1-0123-ABCD-AA1234567895");
        private static readonly Guid ProfRigger = new("A1A1A1A1-A1A1-0123-ABCD-AA1234567896");

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
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProfessionResourceNorms_ResourcesShipbuilding_ResourceId",
                        column: x => x.ResourceId,
                        principalTable: "ResourcesShipbuilding",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProfessionResourceNorms_ProfessionId",
                table: "ProfessionResourceNorms",
                column: "ProfessionId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfessionResourceNorms_ResourceId",
                table: "ProfessionResourceNorms",
                column: "ResourceId");

            //SeedData
            var seedData = new object[,]
            {
                { Guid.NewGuid(), ProfCarpenter, ResourceRaft, 40, 1 },
                { Guid.NewGuid(), ProfShipyardWorker, ResourceRaft, 20, 1 },

                { Guid.NewGuid(), ProfDesigner, ResourceRowingBoat, 15, 1 },
                { Guid.NewGuid(), ProfCarpenter, ResourceRowingBoat, 60, 1 },
                { Guid.NewGuid(), ProfWelder, ResourceRowingBoat, 5, 1 },
                { Guid.NewGuid(), ProfPainter, ResourceRowingBoat, 10, 1 },

                { Guid.NewGuid(), ProfDesigner, ResourceLongRangeShip, 120, 1 },
                { Guid.NewGuid(), ProfWelder, ResourceLongRangeShip, 200, 1 },
                { Guid.NewGuid(), ProfShipyardWorker, ResourceLongRangeShip, 300, 1 },
                { Guid.NewGuid(), ProfRigger, ResourceLongRangeShip, 80, 1 },
                { Guid.NewGuid(), ProfPainter, ResourceLongRangeShip, 50, 1 }
            };

            migrationBuilder.InsertData(
                table: "ProfessionResourceNorms",
                columns: new[] { "Id", "ProfessionId", "ResourceId", "Hours", "QuantityProduced" },
                values: seedData);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProfessionResourceNorms");
        }
    }
}
