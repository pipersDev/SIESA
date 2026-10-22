namespace Api.Endpoints;

using Application.DTOs;
using Domain.Entities;
using Application.Services;
using Infrastructure.Data;

public static class OrderEndpoints
{
    private static readonly AppDbContext _context;
    public static void MapOrderEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/orders").WithTags("Orders");

        group.MapPost("/", async (CreateOrderRequest request, OrderService orderService, CancellationToken ct) =>
        {
            var order = await orderService.CreateOrderAsync(request, ct);
            return Results.Created($"/api/orders/{order.Id}", order);
        })
        .WithName("CreateOrder")
        .Produces<Order>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/", async (OrderService orderService,  CancellationToken ct) =>
        {
            var orders = await orderService.GetAllOrdersAsync(ct);
            return Results.Ok(orders);
        })
        .WithName("GetAllOrders")
        .Produces<List<Order>>(StatusCodes.Status200OK);

        group.MapGet("/{id:int}", async (int id, OrderService orderService, CancellationToken ct) =>
        {
            var order = await orderService.GetOrderByIdAsync(id, ct);
            return order is not null ? Results.Ok(order) : Results.NotFound();
        })
        .WithName("GetOrderById")
        .Produces<Order>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);
    }
}