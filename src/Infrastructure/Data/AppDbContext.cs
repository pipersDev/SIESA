namespace Infrastructure.Data;

using Domain;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<Order> Orders => Set<Order>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Datos semilla iniciales para probar la API fácilmente desde Swagger
        modelBuilder.Entity<Product>().HasData(
            new Product { Id = 1, Name = "Laptop Gamer Pro", Price = 1500.00m, Stock = 5, Version = 1 },
            new Product { Id = 2, Name = "Teclado Mecánico RGB", Price = 85.50m, Stock = 10, Version = 1 },
            new Product { Id = 3, Name = "Mouse Inalámbrico", Price = 45.00m, Stock = 0, Version = 1 } // Sin stock para probar validación
        );
    }
}