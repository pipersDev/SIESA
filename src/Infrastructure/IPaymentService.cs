using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure
{
    public interface IPaymentService
    {
        Task<bool> ProcessPaymentAsync(int orderId, decimal amount, CancellationToken cancellationToken);
    }
}
