using Microsoft.EntityFrameworkCore;
using Sales.Domain;

namespace Sales.Infra
{
    public class SaleDbContext : DbContext
    {
        public SaleDbContext(DbContextOptions<SaleDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Mark SaleItem as keyless
            // Configure the one-to-many relationship between Sale and SaleItem
            modelBuilder.Entity<Sale>()
                .HasMany(s => s.Items) // Sale has many SaleItems
                .WithOne(si => si.Sale) // SaleItem has one Sale
                .HasForeignKey(si => si.SaleId); // Foreign key in SaleItem to Sale

            // Ensure SaleItem has a primary key
            modelBuilder.Entity<SaleItem>()
                .HasKey(si => si.Id); // Ensure SaleItem has a key
        }

        public DbSet<Sale> Sales { get; set; }
        public DbSet<SaleItem> SaleItems { get; set; }
    }

}