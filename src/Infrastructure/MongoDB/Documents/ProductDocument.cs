using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EcommerceDatabaseBenchmark.Infrastructure.MongoDB.Documents;

/// <summary>
/// MongoDB document representing a product.
/// </summary>
public sealed class ProductDocument
{
    [BsonId] public ObjectId Id { get; set; }
    public int ProductId { get; set; }
    public string Sku { get; set; } = default!;
    public string Name { get; set; } = default!;
    public decimal Price { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
