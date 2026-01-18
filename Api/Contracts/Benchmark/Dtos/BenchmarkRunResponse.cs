using EcommerceDatabaseBenchmark.Application.Contracts.Benchmark.Dtos;

namespace EcommerceDatabaseBenchmark.Api.Contracts.Benchmark.Dtos
{
    /// <summary>
    /// API response returned after running a benchmark,
    /// containing which provider was used, how many iterations were executed,
    /// the exact input parameters, and the aggregated benchmark results
    /// for each executed use case.
    /// </summary>
    public sealed record BenchmarkRunResponse(
        string Provider,
        int Iterations,
        BenchmarkInputs Inputs,
        IReadOnlyList<BenchmarkSummary> Results);
}
