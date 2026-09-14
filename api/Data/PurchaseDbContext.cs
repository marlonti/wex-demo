using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Data;

public class PurchaseDbContext(DbContextOptions<PurchaseDbContext> options) : DbContext(options)
{
    public DbSet<Purchase> Purchases => Set<Purchase>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Purchase>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Description).HasMaxLength(50).IsRequired();
            entity.Property(p => p.Amount).HasPrecision(18, 2);
        });
    }
}
