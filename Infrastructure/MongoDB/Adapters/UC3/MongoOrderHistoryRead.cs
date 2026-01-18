using EcommerceDatabaseBenchmark.Application.Dtos.UC3;
using EcommerceDatabaseBenchmark.Application.Interfaces.UC3;
using EcommerceDatabaseBenchmark.Infrastructure.MongoDB.Documents;
using MongoDB.Driver;

namespace EcommerceDatabaseBenchmark.Infrastructure.MongoDB.Adapters.UC3;

/// <summary>
/// MongoDB adapter for UC3 (Customer Order History with paging) that implements IOrderHistoryRead
/// by reading paged order data from the "orders" collection.
///
/// This read path is modeled as:
/// 1) Filter orders by CustomerId.
/// 2) Sort by CreatedAt descending and then OrderId descending to ensure stable ordering
///    across pages.
/// 3) Apply skip/limit based paging using pageNumber and pageSize.
/// 4) Map each order document to a OrderHistoryItem DTO and return an OrderHistoryPage.
///
/// Paging and validation behavior:
/// - pageNumber must be >= 1.
/// - pageSize must be between 1 and 200.
/// </summary>
public sealed class MongoOrderHistoryRead(MongoDb db) : IOrderHistoryRead
{
    public async Task<OrderHistoryPage> GetPageAsync(int customerId, int pageNumber, int pageSize, CancellationToken ct = default)
    {
        if (pageNumber <= 0) throw new InvalidOperationException("pageNumber must be >= 1");
        if (pageSize <= 0 || pageSize > 200) throw new InvalidOperationException("pageSize out of range");

        var orders = db.Database.GetCollection<OrderDocument>("orders");

        var filter = Builders<OrderDocument>.Filter.Eq(x => x.CustomerId, customerId);

        var skip = (pageNumber - 1) * pageSize;

        var docs = await orders.Find(filter)
            .SortByDescending(x => x.CreatedAt)
            .ThenByDescending(x => x.OrderId)
            .Skip(skip)
            .Limit(pageSize)
            .ToListAsync(ct);

        var items = docs.Select(o => new OrderHistoryItem(
            OrderId: o.OrderId,
            CreatedAt: o.CreatedAt,
            Status: o.Status.ToString(),
            TotalAmount: o.TotalAmount
        )).ToList();

        return new OrderHistoryPage(
            CustomerId: customerId,
            PageNumber: pageNumber,
            PageSize: pageSize,
            Orders: items
        );
    }
}
