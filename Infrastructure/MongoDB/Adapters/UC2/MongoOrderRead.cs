using EcommerceDatabaseBenchmark.Application.Contracts.Dtos.UC2;
using EcommerceDatabaseBenchmark.Application.Interfaces.UC2;
using EcommerceDatabaseBenchmark.Infrastructure.MongoDB.Documents;
using MongoDB.Driver;

namespace EcommerceDatabaseBenchmark.Infrastructure.MongoDB.Adapters.UC2;

/// <summary>
/// MongoDB adapter for UC2 (Get Order Details) that implements IOrderRead by assembling an
/// OrderDetails DTO from multiple MongoDB collections.
///
/// This read path is modeled as:
/// 1) Load the order aggregate from the "orders" collection by OrderId, where order items are embedded
///    as subdocuments in OrderDocument.Items. 
/// 2) Load the referenced customer from the "customers" collection by CustomerId.
/// 3) Load the referenced products for all ProductIds found in the order items from the "products"
///    collection
/// 4) Map the embedded order items to OrderItemDetails and return a fully composed OrderDetails.
/// 
/// Data integrity behavior:
/// - If the order does not exist, GetDetailsAsync returns null.
/// - If the referenced customer or any referenced products are missing, placeholder values like "(unknown)"
///   are used instead of failing, to keep benchmark runs stable and comparable.
/// </summary>
public sealed class MongoOrderRead(MongoDb db) : IOrderRead
{
    public async Task<OrderDetails?> GetDetailsAsync(int orderId, CancellationToken ct = default)
    {
        var orders = db.Database.GetCollection<OrderDocument>("orders");
        var customers = db.Database.GetCollection<CustomerDocument>("customers");
        var products = db.Database.GetCollection<ProductDocument>("products");

        // 1) Load order
        var order = await orders.Find(x => x.OrderId == orderId)
            .FirstOrDefaultAsync(ct);

        if (order is null) return null;

        // 2) Load customer (reference)
        var customerDoc = await customers.Find(x => x.CustomerId == order.CustomerId)
            .FirstOrDefaultAsync(ct);

        var customer = customerDoc is null
            ? new CustomerSummary(order.CustomerId, "(unknown)", "(unknown)", "(unknown)")
            : new CustomerSummary(customerDoc.CustomerId, customerDoc.FirstName, customerDoc.LastName, customerDoc.Email);

        // 3) Load products used in the order items
        var productIds = order.Items.Select(i => i.ProductId).Distinct().ToArray();

        var productList = await products
            .Find(Builders<ProductDocument>.Filter.In(x => x.ProductId, productIds))
            .Project(p => new { p.ProductId, p.Sku, p.Name })
            .ToListAsync(ct);

        var productMap = productList.ToDictionary(x => x.ProductId, x => x);

        // 4) Map items to DTO
        var items = order.Items.Select(i =>
        {
            productMap.TryGetValue(i.ProductId, out var p);

            return new OrderItemDetails(
                ProductId: i.ProductId,
                Sku: p?.Sku ?? "(unknown)",
                Name: p?.Name ?? "(unknown)",
                Quantity: i.Quantity,
                UnitPrice: i.UnitPrice
            );
        }).ToList();

        // 5) Map order to DTO
        return new OrderDetails(
            OrderId: order.OrderId,
            CustomerId: order.CustomerId,
            Status: order.Status.ToString(),
            TotalAmount: order.TotalAmount,
            CreatedAt: order.CreatedAt,
            Customer: customer,
            OrderItems: items
        );
    }
}
