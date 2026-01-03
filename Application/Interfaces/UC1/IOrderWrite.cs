using EcommerceDatabaseBenchmark.Domain.Entities;

namespace EcommerceDatabaseBenchmark.Application.Interfaces.UC1
{
    // Port for writing orders
    public interface IOrderWrite
    {
        Task<int> CreateAsync(Order order, CancellationToken ct = default);
    }
}
