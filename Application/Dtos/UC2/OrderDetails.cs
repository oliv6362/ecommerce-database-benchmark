using EcommerceDatabaseBenchmark.Application.Dtos.UC2;

namespace EcommerceDatabaseBenchmark.Application.Dtos.UC2
{
    public sealed record OrderDetails(
        int OrderId,
        int CustomerId,
        string Status,
        decimal TotalAmount,
        DateTimeOffset CreatedAt,
        CustomerSummary Customer,
        IReadOnlyList<OrderItemDetails> OrderItems);
}
