namespace Api.Endpoints;

using Infrastructure.Data;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

public static class ProductEndpoints
{
    public static void MapProductEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/products").WithTags("Products");

        // Endpoint para consultar todos los productos y ver su stock disponible
        group.MapGet("/", async (AppDbContext context, CancellationToken ct) =>
        {
            var products = await context.Products.ToListAsync(ct);
            return Results.Ok(products);
        })
        .WithName("GetAllProducts")
        .Produces<List<Product>>(StatusCodes.Status200OK);
    }
}