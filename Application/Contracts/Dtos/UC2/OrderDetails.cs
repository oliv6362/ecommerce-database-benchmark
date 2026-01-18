namespace EcommerceDatabaseBenchmark.Application.Contracts.Dtos.UC2
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
