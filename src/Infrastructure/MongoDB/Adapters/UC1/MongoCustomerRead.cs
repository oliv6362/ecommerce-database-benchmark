using EcommerceDatabaseBenchmark.Application.Interfaces.UC1;
using EcommerceDatabaseBenchmark.Infrastructure.MongoDB.Documents;
using MongoDB.Driver;

namespace EcommerceDatabaseBenchmark.Infrastructure.MongoDB.Adapters.UC1;

/// <summary>
/// MongoDB adapter responsible for retrieving customer data (Id).
/// </summary>
public sealed class MongoCustomerRead(MongoDb db) : ICustomerRead
{
    public async Task<bool> ExistsAsync(int customerId, CancellationToken ct = default)
    {
        var customers = db.Customers();

        return await customers.Find(x => x.CustomerId == customerId)
            .Limit(1)
            .AnyAsync(ct);
    }
}
