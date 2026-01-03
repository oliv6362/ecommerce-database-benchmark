namespace EcommerceDatabaseBenchmark.Application.Interfaces.UC1
{
    public sealed record ProductSnapshot(int ProductId, decimal Price);

    // Port that is used to fetch product info
    public interface IProductRead
    {
        Task<IReadOnlyList<ProductSnapshot>> GetByIdsAsync(IReadOnlyList<int> productIds, CancellationToken ct = default);
    }
}
