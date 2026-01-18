namespace EcommerceDatabaseBenchmark.Infrastructure.MongoDB
{
    /// <summary>
    /// Configuration options for MongoDB connectivity.
    /// </summary>
    public sealed class MongoOptions
    {
        public string ConnectionString { get; set; } = default!;
        public string Database { get; set; } = default!;
    }
}
