using System.Diagnostics;
using EcommerceDatabaseBenchmark.Application.Usecases.UC1;
using EcommerceDatabaseBenchmark.Application.Usecases.UC2;
using EcommerceDatabaseBenchmark.Application.Usecases.UC3;
using EcommerceDatabaseBenchmark.Application.Usecases.UC4;
using EcommerceDatabaseBenchmark.Application.Contracts.Benchmark.Dtos;
using EcommerceDatabaseBenchmark.Application.Contracts.Dtos.UC1;

namespace EcommerceDatabaseBenchmark.Application.Benchmarking;

/// <summary>
/// Orchestrates performance benchmarks for the main use cases (UC1–UC4)
/// by repeatedly executing them and measuring execution time.
/// </summary>
public sealed class BenchmarkRunner
{
    private readonly PlaceOrderUseCase _uc1;
    private readonly GetOrderDetailsUseCase _uc2;
    private readonly GetCustomerOrderHistoryUseCase _uc3;
    private readonly GetTopSellingProductsUseCase _uc4;

    public BenchmarkRunner(PlaceOrderUseCase uc1, GetOrderDetailsUseCase uc2, GetCustomerOrderHistoryUseCase uc3, GetTopSellingProductsUseCase uc4)
    {
        _uc1 = uc1;
        _uc2 = uc2;
        _uc3 = uc3;
        _uc4 = uc4;
    }


    /// <summary>
    /// Benchmarks the "Place Order" use case by executing it multiple times
    /// with the same request and measuring how long each execution takes.
    /// 
    /// A single warmup run is executed first to reduce cold-start effects.
    /// </summary>
    public async Task<BenchmarkSummary> BenchmarkUC1Async(string name, PlaceOrderRequest request, int iterations, CancellationToken ct = default)
    {
        // Warmup
        await _uc1.ExecuteAsync(request, ct);

        var results = new List<long>(iterations);

        for (int i = 0; i < iterations; i++)
        {
            var sw = Stopwatch.StartNew();
            await _uc1.ExecuteAsync(request, ct);
            sw.Stop();

            results.Add(sw.ElapsedTicks);
        }

        return Summarize(name, results);
    }

    /// <summary>
    /// Benchmarks the "Get Order Details" use case by repeatedly loading
    /// the same order by id and recording the execution time.
    /// 
    /// Includes a warmup call before measurements start.
    /// </summary>
    public async Task<BenchmarkSummary> BenchmarkUC2Async(int orderId, int iterations, CancellationToken ct = default)
    {
        // Warmup
        await _uc2.ExecuteAsync(orderId, ct);

        var results = new List<long>(iterations);

        for (int i = 0; i < iterations; i++)
        {
            var sw = Stopwatch.StartNew();
            await _uc2.ExecuteAsync(orderId, ct);
            sw.Stop();

            results.Add(sw.ElapsedTicks);
        }

        return Summarize("UC2.GetOrderDetails", results);
    }

    /// <summary>
    /// Benchmarks the "Customer Order History" use case by repeatedly
    /// loading the same page of a customer's order history.
    /// 
    /// This is mainly used to compare paging performance
    /// (e.g. page 1 vs page 10).
    /// </summary>
    public async Task<BenchmarkSummary> BenchmarkUC3Async(int customerId, int pageNumber, int pageSize, int iterations, CancellationToken ct = default)
    {
        // Warmup
        await _uc3.ExecuteAsync(customerId, pageNumber, pageSize, ct); 

        var results = new List<long>(iterations);

        for (int i = 0; i < iterations; i++)
        {
            var sw = Stopwatch.StartNew();
            await _uc3.ExecuteAsync(customerId, pageNumber, pageSize, ct);
            sw.Stop();

            results.Add(sw.ElapsedTicks);
        }

        return Summarize($"UC3.OrderHistory.Page{pageNumber}", results);
    }

    /// <summary>
    /// Benchmarks the "Top Selling Products" use case by repeatedly
    /// running the same aggregation query for a given time range and limit.
    /// 
    /// Used to measure performance of analytical/aggregation workloads.
    /// </summary>
    public async Task<BenchmarkSummary> BenchmarkUC4Async( int lastDays, int limit, int iterations, CancellationToken ct = default)
    {
        // Warmup
        await _uc4.ExecuteAsync(lastDays, limit, ct);

        var results = new List<long>(iterations);

        for (int i = 0; i < iterations; i++)
        {
            var sw = Stopwatch.StartNew();
            await _uc4.ExecuteAsync(lastDays, limit, ct);
            sw.Stop();

            results.Add(sw.ElapsedTicks);
        }

        return Summarize("UC4.TopSellingProducts", results);
    }

    /// <summary>
    /// Aggregates raw timing results into statistical metrics such as
    /// min, max, average, p50, p95, and standard deviation.
    /// </summary>
    private static BenchmarkSummary Summarize(string name, IReadOnlyList<long> ticks)
    {
        var sorted = ticks.OrderBy(x => x).ToArray();

        static long Percentile(long[] arr, double p)
        {
            var idx = (int)Math.Ceiling(p * arr.Length) - 1;
            idx = Math.Clamp(idx, 0, arr.Length - 1);
            return arr[idx];
        }

        var avgTicks = ticks.Average();
        var variance = ticks
            .Select(t => (t - avgTicks) * (t - avgTicks))
            .Average();

        return new BenchmarkSummary(
            UseCase: name,
            Iterations: ticks.Count,
            MinMs: TicksToMs(sorted.First()),
            MaxMs: TicksToMs(sorted.Last()),
            AvgMs: TicksToMs((long)avgTicks),
            P50Ms: TicksToMs(Percentile(sorted, 0.50)),
            P95Ms: TicksToMs(Percentile(sorted, 0.95)),
            StdDevMs: Math.Sqrt(variance) * 1000.0 / Stopwatch.Frequency
        );
    }

    /// <summary>
    /// Converts stopwatch ticks to milliseconds.
    /// </summary>
    private static long TicksToMs(long ticks)
    {
        return (long)Math.Round(ticks * 1000.0 / Stopwatch.Frequency);
    }
}
