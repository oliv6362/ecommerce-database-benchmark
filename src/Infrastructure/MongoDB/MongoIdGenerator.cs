using EcommerceDatabaseBenchmark.Infrastructure.MongoDB.Documents;
using MongoDB.Driver;

namespace EcommerceDatabaseBenchmark.Infrastructure.MongoDB;

/// <summary>
/// Generates sequential numeric identifiers in MongoDB using a counter document.
///
/// MongoDB does not provide SQL-style identity columns for incremental integers.
/// This generator implements the common "counters collection" pattern.
/// </summary>
public sealed class MongoIdGenerator(MongoDb db)
{
    public async Task<int> NextOrderIdAsync(CancellationToken ct = default)
    {
        var counters = db.Database.GetCollection<CounterDocument>("counters");

        var updated = await counters.FindOneAndUpdateAsync(
            filter: Builders<CounterDocument>.Filter.Eq(x => x.Id, "orderId"),
            update: Builders<CounterDocument>.Update.Inc(x => x.Value, 1),
            options: new FindOneAndUpdateOptions<CounterDocument>
            {
                IsUpsert = true,
                ReturnDocument = ReturnDocument.After
            },
            cancellationToken: ct);

        return updated.Value;
    }
}
