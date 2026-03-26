using EcommerceDatabaseBenchmark.Application.Interfaces.UC1;
using EcommerceDatabaseBenchmark.Infrastructure.MongoDB.Documents;
using MongoDB.Driver;

namespace EcommerceDatabaseBenchmark.Infrastructure.MongoDB.Adapters.UC1;

/// <summary>
/// MongoDB adapter responsible for retrieving product data (Ids & price).
/// </summary>
public sealed class MongoProductRead(MongoDb db) : IProductRead
{
    public async Task<IReadOnlyList<ProductSnapshot>> GetByIdsAsync(IReadOnlyList<int> productIds, CancellationToken ct = default)
    {
        var products = db.Products();

        var filter = Builders<ProductDocument>.Filter.In(x => x.ProductId, productIds);

        var list = await products.Find(filter)
            .Project(p => new ProductSnapshot(p.ProductId, p.Price))
            .ToListAsync(ct);

        return list;
    }
}
