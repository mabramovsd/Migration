using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migration.Shipbuilding.Migrations
{
    /// <inheritdoc />
    public partial class TableEmployeeProfessions : Microsoft.EntityFrameworkCore.Migrations.Migration
    {
        // ============================================================
        // EMPLOYEE GUIDs (Shipbuilding)
        // ============================================================
        private static readonly Guid EmpWelderCarpenter = new("e1111111-1111-1111-1111-111111111111");
        private static readonly Guid EmpWelder = new("e1111111-1111-1111-1111-111111111112");
        private static readonly Guid EmpCarpenter = new("e1111111-1111-1111-1111-111111111113");
        private static readonly Guid EmpRiggerShipyard = new("e1111111-1111-1111-1111-111111111114");
        private static readonly Guid EmpMulti = new("e1111111-1111-1111-1111-111111111115");
        private static readonly Guid EmpOriginal = new("c1b2c3d4-e5f6-7890-abcd-ef1234567890");

        // ============================================================
        // PROFESSION GUIDs (Shipbuilding)
        // ============================================================
        private static readonly Guid ProfAll = new("A1A1A1A1-A1A1-0123-ABCD-AA1234567890");
        private static readonly Guid ProfCarpenter = new("A1A1A1A1-A1A1-0123-ABCD-AA1234567891");
        private static readonly Guid ProfDesigner = new("A1A1A1A1-A1A1-0123-ABCD-AA1234567892");
        private static readonly Guid ProfWelder = new("A1A1A1A1-A1A1-0123-ABCD-AA1234567893");
        private static readonly Guid ProfShipyardWorker = new("A1A1A1A1-A1A1-0123-ABCD-AA1234567894");
        private static readonly Guid ProfPainter = new("A1A1A1A1-A1A1-0123-ABCD-AA1234567895");
        private static readonly Guid ProfRigger = new("A1A1A1A1-A1A1-0123-ABCD-AA1234567896");

        // ============================================================
        // EMPLOYEE-PROFESSION LINK GUIDs
        // ============================================================
        private static readonly Guid EpEmp1Welder = new("f1111111-1111-1111-1111-111111111111");
        private static readonly Guid EpEmp1Carpenter = new("f1111111-1111-1111-1111-111111111112");
        private static readonly Guid EpEmp2Welder = new("f1111111-1111-1111-1111-111111111113");
        private static readonly Guid EpEmp3Carpenter = new("f1111111-1111-1111-1111-111111111114");
        private static readonly Guid EpEmp4Rigger = new("f1111111-1111-1111-1111-111111111115");
        private static readonly Guid EpEmp4ShipyardWorker = new("f1111111-1111-1111-1111-111111111116");
        private static readonly Guid EpEmp5Welder = new("f1111111-1111-1111-1111-111111111117");
        private static readonly Guid EpEmp5Carpenter = new("f1111111-1111-1111-1111-111111111118");
        private static readonly Guid EpEmp5Designer = new("f1111111-1111-1111-1111-111111111119");
        private static readonly Guid EpOriginalWelder = new("f1111111-1111-1111-1111-111111111120");
        private static readonly Guid EpOriginalDesigner = new("f1111111-1111-1111-1111-111111111121");

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmployeeProfessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProfessionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HireDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FireDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeProfessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeProfessions_Professions_ProfessionId",
                        column: x => x.ProfessionId,
                        principalTable: "Professions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeProfessions_ProfessionId",
                table: "EmployeeProfessions",
                column: "ProfessionId");

            // ============================================================
            // SEED: EmployeesShipbuilding
            // ============================================================
            migrationBuilder.InsertData(
                table: "EmployeesShipbuilding",
                columns: new[] { "Id", "CanWeld", "CanCarpentry", "CanDesignShip", "CanShipyard", "CanPaint", "CanRig", "IsDeleted" },
                values: new object[,]
                {
                    { EmpWelderCarpenter,       true, true, false, false, false, false, false },
                    { EmpWelder,                true, false, false, false, false, false, false },
                    { EmpCarpenter,             false, true, false, false, false, false, false },
                    { EmpRiggerShipyard,        false, false, false, true, false, true, false },
                    { EmpMulti,                 true, true, true, false, false, false, false },
                });

            // ============================================================
            // SEED: EmployeeProfessions
            // ============================================================
            migrationBuilder.InsertData(
                table: "EmployeeProfessions",
                columns: new[] { "Id", "EmployeeId", "ProfessionId", "HireDate" },
                values: new object[,]
                {
                    { EpEmp1Carpenter,   EmpWelderCarpenter, ProfCarpenter,   new DateTime(2023, 6, 1) },

                    { EpEmp2Welder,      EmpWelder,          ProfWelder,      new DateTime(2025, 3, 10) },

                    { EpEmp3Carpenter,   EmpCarpenter,       ProfCarpenter,   new DateTime(2024, 8, 20) },

                    { EpEmp4Rigger,      EmpRiggerShipyard,  ProfRigger,      new DateTime(2025, 1, 5) },

                    { EpEmp5Designer,    EmpMulti, ProfDesigner,    new DateTime(2024, 6, 15) },

                    { EpOriginalWelder,  EmpOriginal,        ProfWelder,      new DateTime(2022, 9, 1) },
                    
                    { EpOriginalDesigner, EmpOriginal,       ProfDesigner,    new DateTime(2023, 2, 1) },
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "EmployeeProfessions");
        }
    }
}
