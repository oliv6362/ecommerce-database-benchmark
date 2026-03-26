namespace EcommerceDatabaseBenchmark.Application.Benchmarking;

public enum BenchmarkProfileName { Small, Medium }

public sealed record BenchmarkProfile(BenchmarkProfileName Name, int Customers, int Products, int Orders, int MaxItemsPerOrder);

/// <summary>
/// Provides predefined dataset configurations used for benchmarking.
/// 
/// Each benchmark profile defines how many customers, products, and orders
/// should be generated, as well as the maximum number of items per order.
/// 
/// These profiles are used to:
/// - Control dataset size for small vs. medium benchmark runs
/// - Ensure repeatable and comparable benchmarks across providers
/// - Centralize benchmark sizing logic in one place
/// </summary>
public static class BenchmarkProfiles
{
    public static BenchmarkProfile Get(BenchmarkProfileName name) => name switch
    {
        BenchmarkProfileName.Small => new BenchmarkProfile(name, 100, 100, 1_000, 10),
        BenchmarkProfileName.Medium => new BenchmarkProfile(name, 1_000, 1_000, 10_000, 10),
        _ => throw new InvalidOperationException("Unknown profile")
    };
}
