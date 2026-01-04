using EcommerceDatabaseBenchmark.Infrastructure.MongoDB.Documents;
using MongoDB.Driver;

namespace EcommerceDatabaseBenchmark.Infrastructure.MongoDB;

/// <summary>
/// Extension methods for accessing MongoDB collections.
/// Helps avoid repetitive collection name strings throughout the codebase.
/// </summary>
public static class MongoCollections
{
    public static IMongoCollection<OrderDocument> Orders(this MongoDb db)
        => db.Database.GetCollection<OrderDocument>("orders");

    public static IMongoCollection<CustomerDocument> Customers(this MongoDb db)
        => db.Database.GetCollection<CustomerDocument>("customers");

    public static IMongoCollection<ProductDocument> Products(this MongoDb db)
        => db.Database.GetCollection<ProductDocument>("products");
}
