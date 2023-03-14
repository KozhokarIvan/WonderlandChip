using Microsoft.EntityFrameworkCore;
using WonderlandChip.Database.Models;

namespace WonderlandChip.Database.DbContexts
{
    public class ChipizationDbContext : DbContext
    {
        public DbSet<Animal> Animals { get; set; } = null!;
        public DbSet<AnimalType> AnimalTypes { get; set; } = null!;
        public DbSet<LocationPoint> LocationPoints { get; set; } = null!;
        public DbSet<Account> Accounts { get; set; } = null!;
        public DbSet<AnimalVisitedLocation> AnimalsVisitedLocations { get; set; } = null!;
        public ChipizationDbContext(DbContextOptions<ChipizationDbContext> options) : base(options)
        {

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Account>()
                .HasIndex(a => a.Email)
                .IsUnique();
            modelBuilder.Entity<Animal>()
                .HasMany(a => a.VisitedLocations)
                .WithOne(avl => avl.Animal)
                .HasForeignKey(avl => avl.AnimalId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<LocationPoint>()
                .HasMany(lp => lp.AnimalVisitedLocations)
                .WithOne(avl => avl.LocationPoint)
                .HasForeignKey(avl => avl.LocationPointId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
