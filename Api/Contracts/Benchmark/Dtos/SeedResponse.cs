namespace EcommerceDatabaseBenchmark.Api.Contracts.Benchmark.Dtos
{
    /// <summary>
    /// API response returned after seeding the database,
    /// describing which provider and dataset profile were used,
    /// the random seed, dataset sizes, and suggested identifiers
    /// to use for UC2 (order details) and UC3 (order history)
    /// to ensure meaningful benchmark scenarios.
    /// </summary>
    public sealed record SeedResponse(
        string Provider,
        string Profile,
        int Seed,
        int Customers,
        int Products,
        int Orders,
        int MaxItemsPerOrder,
        int SuggestedCustomerIdForUC3,
        int SuggestedOrderIdForUC2);
}
