using EcommerceDatabaseBenchmark.Application.Contracts.Dtos.UC4;

namespace EcommerceDatabaseBenchmark.Application.Interfaces.UC4
{
    // Port for fetching top-selling products by quantity within a date range.
    public interface ITopProductsRead
    {
        Task<TopProductsResult> GetTopSellingAsync(DateTimeOffset fromUtc, DateTimeOffset toUtc, int limit, CancellationToken ct = default);
    }
}
