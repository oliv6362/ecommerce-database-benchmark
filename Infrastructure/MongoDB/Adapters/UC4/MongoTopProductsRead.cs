using EcommerceDatabaseBenchmark.Application.Dtos.UC4;
using EcommerceDatabaseBenchmark.Application.Interfaces.UC4;
using EcommerceDatabaseBenchmark.Infrastructure.MongoDB.Documents;
using MongoDB.Bson;
using MongoDB.Driver;

namespace EcommerceDatabaseBenchmark.Infrastructure.MongoDB.Adapters.UC4;

/// <summary>
/// MongoDB adapter for UC4 (Top Selling Products) that implements ITopProductsRead
/// by running an aggregation pipeline over the "orders" collection.
///
/// This read path is modeled as:
/// 1) Match orders by CreatedAt within the [fromUtc, toUtc) time window.
/// 2) Unwind embedded order items so each product occurrence becomes a separate row.
/// 3) Group by ProductId and sum item quantities to calculate total units sold per product.
/// 4) Sort by QuantitySold descending and apply limit.
/// 5) Lookup referenced product data (Sku and Name) from the "products" collection.
/// 6) Project the final shape, using "(unknown)" when referenced product data is missing.
/// 7) Map the aggregation result to TopProductItem DTOs and return a TopProductsResult.
///
/// Validation and integrity behavior:
/// - limit must be between 1 and 1000.
/// </summary>
public sealed class MongoTopProductsRead(MongoDb db) : ITopProductsRead
{
    public async Task<TopProductsResult> GetTopSellingAsync(DateTimeOffset fromUtc, DateTimeOffset toUtc, int limit, CancellationToken ct = default)
    {
        if (limit <= 0 || limit > 1000)
            throw new InvalidOperationException("limit out of range");

        var orders = db.Database.GetCollection<OrderDocument>("orders");

        var match = Builders<OrderDocument>.Filter.Gte(x => x.CreatedAt, fromUtc)
                  & Builders<OrderDocument>.Filter.Lt(x => x.CreatedAt, toUtc);

        var pipeline = orders.Aggregate()
            .Match(match)
            .Unwind(o => o.Items)
            .Group(new BsonDocument
            {
                { "_id", "$Items.ProductId" },
                { "QuantitySold", new BsonDocument("$sum", "$Items.Quantity") }
            })
            .Sort(new BsonDocument("QuantitySold", -1))
            .Limit(limit)
            .Lookup(
                foreignCollectionName: "products",
                localField: "_id",
                foreignField: "ProductId",
                @as: "product")
            .Project(new BsonDocument
            {
                { "ProductId", "$_id" },
                { "QuantitySold", "$QuantitySold" },
                { "Sku", new BsonDocument("$ifNull", new BsonArray { new BsonDocument("$arrayElemAt", new BsonArray { "$product.Sku", 0 }), "(unknown)" }) },
                { "Name", new BsonDocument("$ifNull", new BsonArray { new BsonDocument("$arrayElemAt", new BsonArray { "$product.Name", 0 }), "(unknown)" }) }
            });

        var rows = await pipeline.ToListAsync(ct);

        var items = rows.Select(r => new TopProductItem(
            ProductId: r["ProductId"].AsInt32,
            Sku: r["Sku"].AsString,
            Name: r["Name"].AsString,
            QuantitySold: r["QuantitySold"].ToInt64()
        )).ToList();

        return new TopProductsResult(
            FromUtc: fromUtc,
            ToUtc: toUtc,
            Limit: limit,
            Items: items
        );
    }
}
