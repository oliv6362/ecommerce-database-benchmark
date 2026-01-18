namespace EcommerceDatabaseBenchmark.Application.Contracts.Dtos.UC4
{
    public sealed record TopProductsResult(
        DateTimeOffset FromUtc,
        DateTimeOffset ToUtc,
        int Limit,
        IReadOnlyList<TopProductItem> Items);
}
