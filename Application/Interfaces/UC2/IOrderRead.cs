using EcommerceDatabaseBenchmark.Application.Contracts.Dtos.UC2;

namespace EcommerceDatabaseBenchmark.Application.Interfaces.UC2;

// Port for fetching a order and related details
public interface IOrderRead
{
    Task<OrderDetails?> GetDetailsAsync(int orderId, CancellationToken ct = default);
}
