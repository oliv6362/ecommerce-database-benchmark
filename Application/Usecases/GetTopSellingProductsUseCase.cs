using EcommerceDatabaseBenchmark.Application.Dtos.UC4;
using EcommerceDatabaseBenchmark.Application.Interfaces.UC4;

namespace EcommerceDatabaseBenchmark.Application.UseCases;

/// <summary>
/// Use-case that retrieves top-selling products by quantity sold within a time range. (e.g., last 30 days, top 10 products)
/// 
/// Benchmark focus: 
/// - Measures performance of analytical aggregation queries involving grouping, sorting,
///   and limiting result sets.
/// </summary>
public sealed class GetTopSellingProductsUseCase(ITopProductsRead topProducts)
{
    public Task<TopProductsResult> ExecuteAsync(
        int lastDays = 30,
        int limit = 10,
        CancellationToken ct = default)
    {
        if (lastDays <= 0 || lastDays > 3650)
            throw new InvalidOperationException("lastDays must be between 1 and 3650.");

        if (limit <= 0 || limit > 1000)
            throw new InvalidOperationException("limit must be between 1 and 1000.");

        var toUtc = DateTimeOffset.UtcNow;
        var fromUtc = toUtc.AddDays(-lastDays);

        return topProducts.GetTopSellingAsync(fromUtc, toUtc, limit, ct);
    }
}
