using EcommerceDatabaseBenchmark.Infrastructure.MongoDB.Indexes;

namespace EcommerceDatabaseBenchmark.Extensions;

public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Ensures required MongoDB indexes exist before running the application/benchmarks.
    /// Creates a scoped service provider, resolves MongoIndexInitializer, and invokes
    /// EnsureIndexesAsync to create indexes idempotently.
    /// </summary>
    public static async Task EnsureMongoIndexesAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var mongoIndexes = scope.ServiceProvider.GetRequiredService<MongoIndexInitializer>();
        await mongoIndexes.EnsureIndexesAsync();
    }
}
