namespace EcommerceDatabaseBenchmark.Application.Dtos.UC3
{
    public sealed record OrderHistoryItem(
        int OrderId,
        DateTimeOffset CreatedAt,
        string Status,
        decimal TotalAmount);
}
