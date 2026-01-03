using EcommerceDatabaseBenchmark.Domain.Entities;   

namespace EcommerceDatabaseBenchmark.Application.Interfaces
{
    // Port for writing orders
    public interface IOrderWrite
    {
        Task<int> CreateAsync(Order order, CancellationToken ct = default);
    }
}
