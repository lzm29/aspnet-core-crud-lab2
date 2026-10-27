using Lab2.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Lab2.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("Products");
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Name).IsRequired().HasMaxLength(120);
            entity.Property(p => p.Description).HasMaxLength(500);
            entity.Property(p => p.Price).IsRequired();
            entity.Property(p => p.Quantity).IsRequired();
            entity.Property(p => p.CreatedAt).IsRequired();
        });
    }
}
