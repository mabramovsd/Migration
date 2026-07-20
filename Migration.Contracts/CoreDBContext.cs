using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Migration.Contracts.DTO.Companies;
using Migration.Contracts.DTO.Employees;

namespace Migration.Contracts
{
    //dotnet ef migrations add "Table Empliyees" --context CoreDBContext --project Migration.Contracts
    //Update-Database -context CoreDBContext
    public class CoreDBContext : DbContext
    {
        public CoreDBContext(DbContextOptions<CoreDBContext> options) : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Company> Companies { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }

    public class CoreDBContextFactory : IDesignTimeDbContextFactory<CoreDBContext>
    {
        public CoreDBContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<CoreDBContext>();
            optionsBuilder.UseSqlServer("Data Source=MSI;Initial Catalog=Migration_Core;Integrated Security=True;Trust Server Certificate=True");

            return new CoreDBContext(optionsBuilder.Options);
        }
    }
}
