namespace EcommerceDatabaseBenchmark.Application.Contracts.Benchmark.Dtos
{
    /// <summary>
    /// Defines all input parameters used for a single benchmark run,
    /// including identifiers and paging/limit settings for UC2–UC4.
    /// These values are stored with the results to ensure benchmarks
    /// are reproducible and comparable.
    /// </summary>
    public sealed record BenchmarkInputs(
        int OrderIdForUC2,
        int CustomerIdForUC3,
        int PageSize,
        int Page1,
        int Page10,
        int LastDaysForUC4,
        int TopLimitForUC4);
}
