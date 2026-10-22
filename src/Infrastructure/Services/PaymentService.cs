using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Services
{
    public class PaymentService : IPaymentService
    {
        public async Task<bool> ProcessPaymentAsync(int orderId, decimal amount, CancellationToken cancellationToken)
        {
            // Simular latencia de red
            await Task.Delay(500, cancellationToken);

            // Simular lógica de éxito (ej. 90% de éxito, 10% de fallo o basado en reglas)
            return true;
        }
    }
}
