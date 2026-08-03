using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Migration.Shipbuilding.Entities;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Migration.Shipbuilding
{
    //dotnet ef migrations add "Table EmployeesShipbuilding" --context ShipbuildingDBContext --project Migration.Shipbuilding
    //Update-Database -context ShipbuildingDBContext
    public class ShipbuildingDBContext : DbContext
    {
        public ShipbuildingDBContext(DbContextOptions<ShipbuildingDBContext> options) : base(options)
        {
        }

        public DbSet<EmployeeShipbuilding> EmployeesShipbuilding { get; set; }
        public DbSet<Profession> Professions { get; set; }
        public DbSet<ResourceShipbuilding> ResourcesShipbuilding { get; set; }
        public DbSet<ProfessionResourceNorm> ProfessionResourceNorms { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ProfessionResourceNorm FK relationships
            modelBuilder.Entity<ProfessionResourceNorm>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasOne<Profession>(e => e.Profession)
                    .WithMany()
                    .HasForeignKey(e => e.ProfessionId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne<ResourceShipbuilding>(e => e.Resource)
                    .WithMany()
                    .HasForeignKey(e => e.ResourceId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }

    public class CoreDBContextFactory : IDesignTimeDbContextFactory<ShipbuildingDBContext>
    {
        public ShipbuildingDBContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ShipbuildingDBContext>();
            optionsBuilder.UseSqlServer("Data Source=MSI;Initial Catalog=Migration_Shipbuilding;Integrated Security=True;Trust Server Certificate=True");

            return new ShipbuildingDBContext(optionsBuilder.Options);
        }
    }
}
