namespace EcommerceDatabaseBenchmark.Application.Dtos.UC3
{
    public sealed record OrderHistoryPage(
        int CustomerId,
        int PageNumber,
        int PageSize,
        IReadOnlyList<OrderHistoryItem> Orders);
}
