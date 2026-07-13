using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migration.Shipbuilding.Migrations
{
    /// <inheritdoc />
    public partial class AddSeedDataShip : Microsoft.EntityFrameworkCore.Migrations.Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("INSERT INTO EmployeesShipbuilding (Id, CanWeld, CanCarpentry, CanDesignShip, IsDeleted) VALUES ('c1b2c3d4-e5f6-7890-abcd-ef1234567890', 1, 0, 1, 0)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM EmployeesShipbuilding WHERE Id = 'c1b2c3d4-e5f6-7890-abcd-ef1234567890'");
        }
    }

}
