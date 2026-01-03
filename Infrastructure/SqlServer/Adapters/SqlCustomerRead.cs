using EcommerceDatabaseBenchmark.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EcommerceDatabaseBenchmark.Infrastructure.SqlServer.Adapters
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
