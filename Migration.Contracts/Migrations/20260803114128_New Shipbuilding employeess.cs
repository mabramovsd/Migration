using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migration.Contracts.Migrations
{
    /// <inheritdoc />
    public partial class NewShipbuildingemployeess : Microsoft.EntityFrameworkCore.Migrations.Migration
    {
        // ============================================================
        // EMPLOYEE GUIDs (Shipbuilding)
        // ============================================================
        private static readonly Guid EmpWelderCarpenter = new("e1111111-1111-1111-1111-111111111111");
        private static readonly Guid EmpWelder = new("e1111111-1111-1111-1111-111111111112");
        private static readonly Guid EmpCarpenter = new("e1111111-1111-1111-1111-111111111113");
        private static readonly Guid EmpRiggerShipyard = new("e1111111-1111-1111-1111-111111111114");
        private static readonly Guid EmpMulti = new("e1111111-1111-1111-1111-111111111115");

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "BirthDate", "FullName", "CurrentCompany", "IsDeleted" },
                values: new object[,]
                {
                    { EmpWelderCarpenter, new DateTime(1998, 11, 03), "Олег Олегов", "Shipbuilding", false },
                    { EmpWelder, new DateTime(1998, 11, 03), "Семен Олегов", "Shipbuilding", false },
                    { EmpCarpenter, new DateTime(1998, 11, 03), "Иван Олегов", "Shipbuilding", false },
                    { EmpRiggerShipyard, new DateTime(1998, 11, 03), "Оксана Олегова", "Shipbuilding", false },
                    { EmpMulti, new DateTime(1998, 11, 03), "Егор Егоров", "Shipbuilding", false },
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValues: new object[]
                {
                    EmpWelderCarpenter,
                    EmpWelder,
                    EmpCarpenter,
                    EmpRiggerShipyard,
                    EmpMulti
                });
        }
    }
}
