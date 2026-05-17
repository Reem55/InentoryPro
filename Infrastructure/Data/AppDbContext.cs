using Microsoft.EntityFrameworkCore;
using InventoryAPI.Domain.Entities;

namespace InventoryAPI.Infrastructure.Data;

// ✅ EF Core: DbContext
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ✅ EF Core: Fluent API configuration

        // Product config
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Price).HasPrecision(18, 2);
            entity.Property(e => e.Category).HasMaxLength(50);

            // Relationship: Product → Supplier
            entity.HasOne(e => e.Supplier)
                  .WithMany(s => s.Products)
                  .HasForeignKey(e => e.SupplierId)
                  .OnDelete(DeleteBehavior.Restrict);

            // Index for faster search
            entity.HasIndex(e => e.Category);
            entity.HasIndex(e => e.Name);
        });

        // Supplier config
        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(150);
            entity.HasIndex(e => e.Email).IsUnique();
        });

        // User config
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(150);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.Username).IsUnique();
        });

        // ✅ Seed data
        modelBuilder.Entity<Supplier>().HasData(
            new Supplier { Id = 1, Name = "TechSupplier Co.", Email = "tech@supplier.com", Phone = "+1234567890", CreatedAt = DateTime.UtcNow },
            new Supplier { Id = 2, Name = "Office Depot", Email = "office@depot.com", Phone = "+0987654321", CreatedAt = DateTime.UtcNow }
        );

        modelBuilder.Entity<Product>().HasData(
            new Product { Id = 1, Name = "Laptop", Description = "High-performance laptop", Price = 1200, Quantity = 50, Category = "Electronics", SupplierId = 1, CreatedAt = DateTime.UtcNow },
            new Product { Id = 2, Name = "Mouse", Description = "Wireless mouse", Price = 25, Quantity = 200, Category = "Accessories", SupplierId = 1, CreatedAt = DateTime.UtcNow },
            new Product { Id = 3, Name = "Desk Chair", Description = "Ergonomic chair", Price = 350, Quantity = 30, Category = "Furniture", SupplierId = 2, CreatedAt = DateTime.UtcNow }
        );
    }
}
