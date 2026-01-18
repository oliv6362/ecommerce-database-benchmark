using EcommerceDatabaseBenchmark.Application.Interfaces.UC1;
using EcommerceDatabaseBenchmark.Domain.Entities;
using EcommerceDatabaseBenchmark.Infrastructure.MongoDB.Documents;
using MongoDB.Driver;

namespace EcommerceDatabaseBenchmark.Infrastructure.MongoDB.Adapters.UC1;

/// <summary>
/// MongoDB adapter responsible for persisting orders.
/// </summary>
public sealed class MongoOrderWrite(MongoDb db, MongoIdGenerator ids) : IOrderWrite
{
    public async Task<int> CreateAsync(Order order, CancellationToken ct = default)
    {
        var orders = db.Database.GetCollection<OrderDocument>("orders");

        if (order.OrderId == 0)
            order.OrderId = await ids.NextOrderIdAsync(ct);

        var doc = new OrderDocument
        {
            OrderId = order.OrderId,
            CustomerId = order.CustomerId,
            Status = (int)order.Status,
            TotalAmount = order.TotalAmount,
            CreatedAt = order.CreatedAt,
            Items = order.OrderItems.Select(i => new OrderItemDocument
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            }).ToList()
        };

        await orders.InsertOneAsync(doc, cancellationToken: ct);
        return order.OrderId;
    }
}
