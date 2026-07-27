using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Migration.NurseryHome.DTO;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Migration.NurseryHome
{
    //dotnet ef migrations add "Table EmployeesNurseryHome" --context NurseryHomeDBContext --project Migration.NurseryHome
    //Update-Database -context NurseryHomeDBContext
    public class NurseryHomeDBContext : DbContext
    {
        public NurseryHomeDBContext(DbContextOptions<NurseryHomeDBContext> options) : base(options)
        {
        }

        public DbSet<EmployeeNurseryHome> EmployeesNurseryHome { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }

    public class CoreDBContextFactory : IDesignTimeDbContextFactory<NurseryHomeDBContext>
    {
        public NurseryHomeDBContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<NurseryHomeDBContext>();
            optionsBuilder.UseSqlServer("Data Source=MSI;Initial Catalog=Migration_NurseryHome;Integrated Security=True;Trust Server Certificate=True");

            return new NurseryHomeDBContext(optionsBuilder.Options);
        }
    }
}
