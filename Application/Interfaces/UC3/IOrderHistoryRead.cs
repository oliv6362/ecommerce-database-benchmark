using EcommerceDatabaseBenchmark.Application.Dtos.UC3;

namespace EcommerceDatabaseBenchmark.Application.Interfaces.UC3;

// Port for fetching a paged order history for a customer.
public interface IOrderHistoryRead
{
    Task<OrderHistoryPage> GetPageAsync(
        int customerId,
        int pageNumber,
        int pageSize,
        CancellationToken ct = default);
}
