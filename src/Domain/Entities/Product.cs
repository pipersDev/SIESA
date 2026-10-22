
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }

        [ConcurrencyCheck]
        public int Version { get; set; } = default!; // Clave para prevenir sobreventa por concurrencia optimista
    }
}
