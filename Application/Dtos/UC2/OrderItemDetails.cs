namespace EcommerceDatabaseBenchmark.Application.Dtos.UC2
{
    public sealed record OrderItemDetails(
        int ProductId,
        string Sku,
        string Name,
        int Quantity,
        decimal UnitPrice);
}
