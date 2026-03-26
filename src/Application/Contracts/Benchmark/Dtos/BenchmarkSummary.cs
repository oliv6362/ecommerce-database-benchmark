namespace EcommerceDatabaseBenchmark.Application.Contracts.Benchmark.Dtos;

/// <summary>
/// Contains aggregated timing statistics for a single use case benchmark,
/// calculated from multiple iterations.
/// Includes min, max, average, median (P50), tail latency (P95),
/// and standard deviation, all expressed in milliseconds.
/// </summary>
public sealed record BenchmarkSummary(
    string UseCase,
    int Iterations,
    long MinMs,
    long MaxMs,
    double AvgMs,
    long P50Ms,
    long P95Ms,
    double StdDevMs);
