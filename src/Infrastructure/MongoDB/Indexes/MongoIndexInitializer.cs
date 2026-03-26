using EcommerceDatabaseBenchmark.Infrastructure.MongoDB.Documents;
using MongoDB.Driver;

namespace EcommerceDatabaseBenchmark.Infrastructure.MongoDB.Indexes;

/// <summary>
/// Initializes and ensures that all required MongoDB indexes exists for orders, customers, and products, 
/// which is used by the Use-cases.
/// </summary>
public sealed class MongoIndexInitializer(MongoDb db)
{
    public async Task EnsureIndexesAsync(CancellationToken ct = default)
    {
        var orders = db.Orders();
        var customers = db.Customers();
        var products = db.Products();

        // ORDERS
        // UC2: fetch by OrderId
        await orders.Indexes.CreateOneAsync(
            new CreateIndexModel<OrderDocument>(
                Builders<OrderDocument>.IndexKeys.Ascending(x => x.OrderId),
                new CreateIndexOptions { Unique = true, Name = "ux_orders_orderId" }),
            cancellationToken: ct);

        // UC3: customer order history (CustomerId + CreatedAt desc + OrderId desc)
        var uc3 = Builders<OrderDocument>.IndexKeys
            .Ascending(x => x.CustomerId)
            .Descending(x => x.CreatedAt)
            .Descending(x => x.OrderId);

        await orders.Indexes.CreateOneAsync(
            new CreateIndexModel<OrderDocument>(uc3, new CreateIndexOptions { Name = "ix_orders_customer_createdAt_orderId" }),
            cancellationToken: ct);

        // UC4: time filter on CreatedAt
        var uc4 = Builders<OrderDocument>.IndexKeys.Descending(x => x.CreatedAt);

        await orders.Indexes.CreateOneAsync(
            new CreateIndexModel<OrderDocument>(uc4, new CreateIndexOptions { Name = "ix_orders_createdAt" }),
            cancellationToken: ct);

        // CUSTOMERS
        await customers.Indexes.CreateOneAsync(
            new CreateIndexModel<CustomerDocument>(
                Builders<CustomerDocument>.IndexKeys.Ascending(x => x.CustomerId),
                new CreateIndexOptions { Unique = true, Name = "ux_customers_customerId" }),
            cancellationToken: ct);

        await customers.Indexes.CreateOneAsync(
            new CreateIndexModel<CustomerDocument>(
                Builders<CustomerDocument>.IndexKeys.Ascending(x => x.Email),
                new CreateIndexOptions { Unique = true, Name = "ux_customers_email" }),
            cancellationToken: ct);

        // PRODUCTS
        await products.Indexes.CreateOneAsync(
            new CreateIndexModel<ProductDocument>(
                Builders<ProductDocument>.IndexKeys.Ascending(x => x.ProductId),
                new CreateIndexOptions { Unique = true, Name = "ux_products_productId" }),
            cancellationToken: ct);

        await products.Indexes.CreateOneAsync(
            new CreateIndexModel<ProductDocument>(
                Builders<ProductDocument>.IndexKeys.Ascending(x => x.Sku),
                new CreateIndexOptions { Unique = true, Name = "ux_products_sku" }),
            cancellationToken: ct);
    }
}
