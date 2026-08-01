using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Migration.Agro.Entities;
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
        public DbSet<ResourceAgro> ResourcesAgro { get; set; }
        public DbSet<ProfessionResourceNorm> ProfessionResourceNorms { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ProfessionResourceNorm FK relationships
            modelBuilder.Entity<ProfessionResourceNorm>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasOne<Profession>()
                    .WithMany()
                    .HasForeignKey(e => e.ProfessionId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne<ResourceAgro>()
                    .WithMany()
                    .HasForeignKey(e => e.ResourceId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
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
