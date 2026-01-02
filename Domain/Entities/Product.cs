namespace EcommerceDatabaseBenchmark.Domain.Entities
{
    public class Product
    {
        public int ProductId { get; set; }
        public string Sku { get; set; } = null!;
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
