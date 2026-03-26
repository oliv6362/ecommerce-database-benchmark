namespace EcommerceDatabaseBenchmark.Application.Interfaces.UC1
{
    // Port that validates customer existence
    public interface ICustomerRead
    {
        Task<bool> ExistsAsync(int customerId, CancellationToken ct = default);
    }
}
