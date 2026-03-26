using EcommerceDatabaseBenchmark.Domain.Entities;
using EcommerceDatabaseBenchmark.Infrastructure.MongoDB.Documents;
using MongoDB.Driver;

namespace EcommerceDatabaseBenchmark.Infrastructure.MongoDB.Seed;

/// <summary>
/// MongoDB seeder used to persist deterministic benchmark data as documents.
///
/// Responsibilities:
/// - Store customers, products, and orders in MongoDB collections using the same domain data as the SQL Server seeder.
/// - Convert domain entities (Customer, Product, Order) into MongoDB document types (CustomerDocument, ProductDocument,
///   OrderDocument).
/// - Embed order items inside each order document (OrderDocument.Items) to model the order as an aggregate for reads
///   and aggregations (UC2–UC4).
///
/// Seeding behavior:
/// - SeedAsync clears all benchmark collections first to guarantee a clean baseline for each run.
/// - Customers and products are inserted in bulk via InsertManyAsync.
/// - Orders are inserted in bulk and each order is assigned a sequential OrderId using MongoIdGenerator to keep ids
///   stable and comparable across benchmark runs.
/// - OrderItem documents are created from the order's OrderItems and embedded within the order document.
///
/// Clearing behavior:
/// - ClearAsync deletes all documents from the "orders", "products", and "customers" collections.
/// - The "counters" collection is also cleared to reset the OrderId counter used by MongoIdGenerator.
/// </summary>
public sealed class MongoSeeder
{
    private readonly MongoDb _db;
    private readonly MongoIdGenerator _ids;

    public MongoSeeder(MongoDb db, MongoIdGenerator ids)
    {
        _db = db;
        _ids = ids;
    }

    public async Task SeedAsync(IReadOnlyList<Customer> customers, IReadOnlyList<Product> products, IReadOnlyList<Order> orders, CancellationToken ct = default)
    {
        await ClearAsync(ct);

        await SeedCustomersAsync(customers, ct);
        await SeedProductsAsync(products, ct);
        await SeedOrdersAsync(orders, ct);
    }

    // Customers
    private async Task SeedCustomersAsync( IReadOnlyList<Customer> customers, CancellationToken ct)
    {
        var collection = _db.Customers();

        if (customers.Count == 0) return;

        var docs = customers.Select(c => new CustomerDocument
        {
            CustomerId = c.CustomerId,
            FirstName = c.FirstName,
            LastName = c.LastName,
            Email = c.Email,
            CreatedAt = c.CreatedAt
        }).ToList();

        await collection.InsertManyAsync(docs, cancellationToken: ct);
    }

    // Products
    private async Task SeedProductsAsync(IReadOnlyList<Product> products, CancellationToken ct)
    {
        var collection = _db.Products();

        if (products.Count == 0) return;

        var docs = products.Select(p => new ProductDocument
        {
            ProductId = p.ProductId,
            Sku = p.Sku,
            Name = p.Name,
            Price = p.Price,
            CreatedAt = p.CreatedAt
        }).ToList();

        await collection.InsertManyAsync(docs, cancellationToken: ct);
    }

    // Orders
    private async Task SeedOrdersAsync(IReadOnlyList<Order> orders, CancellationToken ct)
    {
        var collection = _db.Orders();

        if (orders.Count == 0) return;

        var docs = new List<OrderDocument>(orders.Count);

        foreach (var order in orders)
        {
            // Assign sequential OrderId
            order.OrderId = await _ids.NextOrderIdAsync(ct);

            docs.Add(new OrderDocument
            {
                OrderId = order.OrderId,
                CustomerId = order.CustomerId,
                Status = (int)order.Status,
                CreatedAt = order.CreatedAt,
                TotalAmount = order.TotalAmount,
                Items = order.OrderItems.Select(i => new OrderItemDocument
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
            });
        }

        await collection.InsertManyAsync(docs, cancellationToken: ct);
    }

    public async Task ClearAsync(CancellationToken ct = default)
    {
        await _db.Orders().DeleteManyAsync(FilterDefinition<OrderDocument>.Empty, ct);
        await _db.Products().DeleteManyAsync(FilterDefinition<ProductDocument>.Empty, ct);
        await _db.Customers().DeleteManyAsync(FilterDefinition<CustomerDocument>.Empty, ct);

        // Reset counters
        var counters = _db.Database.GetCollection<CounterDocument>("counters");
        await counters.DeleteManyAsync(FilterDefinition<CounterDocument>.Empty, ct);
    }
}
