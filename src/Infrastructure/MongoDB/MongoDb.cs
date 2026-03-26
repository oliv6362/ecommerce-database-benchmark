using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace EcommerceDatabaseBenchmark.Infrastructure.MongoDB;

/// <summary>
/// Wrapper that provides access to a configured MongoDB database.
/// Equivalent to SqlDbContext for SQL Server.
/// </summary>
public sealed class MongoDb
{
    public IMongoDatabase Database { get; }

    public MongoDb(IOptions<MongoOptions> options)
    {
        var cfg = options.Value;

        var client = new MongoClient(cfg.ConnectionString);
        Database = client.GetDatabase(cfg.Database);
    }
}
