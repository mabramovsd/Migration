using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Migration.School.DTO;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Migration.School
{
    //dotnet ef migrations add "Table EmployeesSchool" --context SchoolDBContext --project Migration.School
    //Update-Database -context SchoolDBContext
    public class SchoolDBContext : DbContext
    {
        public SchoolDBContext(DbContextOptions<SchoolDBContext> options) : base(options)
        {
        }

        public DbSet<EmployeeSchool> EmployeesSchool { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }

    public class CoreDBContextFactory : IDesignTimeDbContextFactory<SchoolDBContext>
    {
        public SchoolDBContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<SchoolDBContext>();
            optionsBuilder.UseSqlServer("Data Source=MSI;Initial Catalog=Migration_School;Integrated Security=True;Trust Server Certificate=True");

            return new SchoolDBContext(optionsBuilder.Options);
        }
    }
}
