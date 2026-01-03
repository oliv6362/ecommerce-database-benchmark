using EcommerceDatabaseBenchmark.Application.Dtos.UC2;
using EcommerceDatabaseBenchmark.Application.Interfaces.UC2;

namespace EcommerceDatabaseBenchmark.Application.Usecases.UC2;

/// <summary>
/// Use-case that loads all data needed for an "Order details" page.
/// 
/// Fetches:
/// - Order header (status, total, createdAt)
/// - Customer summary (id, name, email)
/// - Order items with product info (sku, name) and pricing
/// 
/// Benchmark focus:
/// - Measures read performance for a single order by id,
///   including related data retrieval.
/// </summary>
public sealed class GetOrderDetailsUseCase(IOrderRead orders)
{
    public async Task<OrderDetails> ExecuteAsync(int orderId, CancellationToken ct = default)
    {
        if (orderId <= 0)
            throw new InvalidOperationException("OrderId must be greater than 0.");

        var result = await orders.GetDetailsAsync(orderId, ct);

        if (result is null)
            throw new InvalidOperationException("Order not found.");

        return result;
    }
}
