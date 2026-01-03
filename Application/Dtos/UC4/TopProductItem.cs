namespace EcommerceDatabaseBenchmark.Application.Dtos.UC4
{
    public sealed record TopProductItem(
        int ProductId,
        string Sku,
        string Name,
        long QuantitySold);
}
