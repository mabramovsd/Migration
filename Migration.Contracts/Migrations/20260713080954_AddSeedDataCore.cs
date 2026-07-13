using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migration.Contracts.Migrations
{
    /// <inheritdoc />
    public partial class AddSeedDataCore : Microsoft.EntityFrameworkCore.Migrations.Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"
                INSERT INTO Employees 
                    (Id, BirthDate, FullName, CurrentCompany, IsDeleted) 
                VALUES 
                    ('a1b2c3d4-e5f6-7890-abcd-ef1234567890', '1990-05-15', 'Иван Иванов', 'Agro', 0)");

            migrationBuilder.Sql($@"
                INSERT INTO Employees 
                    (Id, BirthDate, FullName, CurrentCompany, IsDeleted) 
                VALUES 
                    ('b2c3d4e5-f6a7-8901-bcde-f12345678901', '1996-12-15', 'Петр Петров', 'Second Company', 0)");

            migrationBuilder.Sql($@"
                INSERT INTO Employees 
                    (Id, BirthDate, FullName, CurrentCompany, IsDeleted) 
                VALUES 
                    ('c1b2c3d4-e5f6-7890-abcd-ef1234567890', '1989-08-06', 'Семен Семенов', 'Shipbuilding', 0)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM Employees WHERE Id = 'a1b2c3d4-e5f6-7890-abcd-ef1234567890'");
            migrationBuilder.Sql("DELETE FROM Employees WHERE Id = 'b2c3d4e5-f6a7-8901-bcde-f12345678901'");
            migrationBuilder.Sql("DELETE FROM Employees WHERE Id = 'c1b2c3d4-e5f6-7890-abcd-ef1234567890'");
        }
    }
}
