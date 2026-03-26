using EcommerceDatabaseBenchmark.Application.Interfaces.UC1;
using Microsoft.EntityFrameworkCore;

namespace EcommerceDatabaseBenchmark.Infrastructure.SqlServer.Adapters.UC1
{
    /// <summary>
    /// SQL Server adapter responsible for retrieving customer data (Id) using Entity Framework Core.
    /// </summary>
    public sealed class SqlCustomerRead(SqlDbContext db) : ICustomerRead
    {
        public Task<bool> ExistsAsync(int customerId, CancellationToken ct = default) => 
            db.Customers.AnyAsync(c => c.CustomerId == customerId, ct);
    }
}
