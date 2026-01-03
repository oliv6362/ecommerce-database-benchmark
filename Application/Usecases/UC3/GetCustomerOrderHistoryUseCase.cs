using EcommerceDatabaseBenchmark.Application.Dtos.UC3;
using EcommerceDatabaseBenchmark.Application.Interfaces.UC3;

namespace EcommerceDatabaseBenchmark.Application.Usecases.UC3;

/// <summary>
/// Use-case that loads paged order history for a customer (latest first).
/// 
/// Benchmark focus: 
/// - Measures read performance for paged order history retrieval. (e.g 1 vs Page 10)
/// </summary>
public sealed class GetCustomerOrderHistoryUseCase(IOrderHistoryRead history)
{
    public Task<OrderHistoryPage> ExecuteAsync(int customerId, int pageNumber, int pageSize, CancellationToken ct = default)
    {
        if (customerId <= 0) throw new InvalidOperationException("CustomerId must be > 0.");
        if (pageNumber <= 0) throw new InvalidOperationException("PageNumber must be >= 1.");
        if (pageSize <= 0 || pageSize > 200) throw new InvalidOperationException("PageSize must be 1..200.");

        return history.GetPageAsync(customerId, pageNumber, pageSize, ct);
    }
}
