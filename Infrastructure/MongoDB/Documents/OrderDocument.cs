using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EcommerceDatabaseBenchmark.Infrastructure.MongoDB.Documents;

/// <summary>
/// MongoDB document representing an order aggregate.
///
/// This document acts as the aggregate root for orders and contains
/// all orderItems as embedded subdocuments.
/// </summary>
public sealed class OrderDocument
{
    [BsonId]
    public ObjectId Id { get; set; }

    public int OrderId { get; set; }
    public int CustomerId { get; set; }
    public int Status { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    public List<OrderItemDocument> Items { get; set; } = new();
}

/// <summary>
/// Embedded subdocument representing a single orderItem.
/// </summary>
public sealed class OrderItemDocument
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
