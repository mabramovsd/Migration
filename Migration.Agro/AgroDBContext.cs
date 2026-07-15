using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Migration.Agro.DTO;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Migration.Agro
{
    //dotnet ef migrations add "Table EmployeesAgro" --context AgroDBContext --project Migration.Agro
    //Update-Database -context AgroDBContext
    public class AgroDBContext : DbContext
    {
        public AgroDBContext(DbContextOptions<AgroDBContext> options) : base(options)
        {
        }

        public DbSet<EmployeeAgro> EmployeesAgro { get; set; }
        public DbSet<Profession> Professions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }

    public class CoreDBContextFactory : IDesignTimeDbContextFactory<AgroDBContext>
    {
        public AgroDBContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AgroDBContext>();
            optionsBuilder.UseSqlServer("Data Source=MSI;Initial Catalog=Migration_Agro;Integrated Security=True;Trust Server Certificate=True");

            return new AgroDBContext(optionsBuilder.Options);
        }
    }
}