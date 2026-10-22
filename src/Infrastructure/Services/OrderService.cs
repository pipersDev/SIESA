namespace Application.Services;

using Application.DTOs;
using Domain.Entities;
using Domain.Enums;
using Infrastructure;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

public class OrderService
{
    private readonly AppDbContext _context;
    private readonly IPaymentService _paymentService;

    public OrderService(AppDbContext context, IPaymentService paymentService)
    {
        _context = context;
        _paymentService = paymentService;
    }

    public async Task<Order> CreateOrderAsync(CreateOrderRequest request, CancellationToken cancellationToken)
    {
        const int maxRetries = 3;
        for (int attempt = 0; attempt < maxRetries; attempt++)
        {
            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var product = await _context.Products.FindAsync(new object[] { request.ProductId }, cancellationToken);
                if (product == null)
                    throw new KeyNotFoundException("El producto especificado no existe.");

                if (product.Stock < request.Quantity)
                    throw new InvalidOperationException("Stock insuficiente. No se puede procesar el pedido (Prevención de sobreventa).");

                // Descontar inventario de forma segura
                product.Stock -= request.Quantity;

                var order = new Order
                {
                    ProductId = product.Id,
                    Quantity = request.Quantity,
                    TotalAmount = product.Price * request.Quantity,
                    Status = OrderStatus.Pending
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync(cancellationToken);

                // Simular integración con proveedor de pagos
                bool paymentSuccess = await _paymentService.ProcessPaymentAsync(order.Id, order.TotalAmount, cancellationToken);

                if (paymentSuccess)
                {
                    order.Status = OrderStatus.Paid;
                }
                else
                {
                    order.Status = OrderStatus.Failed;
                    product.Stock += request.Quantity; // Rollback de inventario si el pago falla
                }

                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                return order;
            }
            catch (DbUpdateConcurrencyException)
            {
                await transaction.RollbackAsync(cancellationToken);
                if (attempt == maxRetries - 1) throw;
                await Task.Delay(50, cancellationToken); // Breve espera antes del reintento por conflicto de concurrencia
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }

        throw new Exception("No se pudo procesar el pedido debido a alta concurrencia en este momento.");
    }

    // Obtener todas las órdenes
    public async Task<List<Order>> GetAllOrdersAsync(CancellationToken ct = default)
    {
        return await _context.Orders.ToListAsync(ct);
    }

    // Obtener orden por ID
    public async Task<Order?> GetOrderByIdAsync(int id, CancellationToken ct = default)
    {
        return await _context.Orders.FindAsync([id], ct);
    }
}