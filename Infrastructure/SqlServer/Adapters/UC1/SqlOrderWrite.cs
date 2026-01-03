using EcommerceDatabaseBenchmark.Application.Interfaces.UC1;
using EcommerceDatabaseBenchmark.Domain.Entities;

namespace EcommerceDatabaseBenchmark.Infrastructure.SqlServer.Adapters.UC1;

/// <summary>
/// SQL Server adapter responsible for persisting orders using Entity Framework Core.
/// Order persistence is executed within an explicit database transaction.
/// </summary>
public sealed class SqlOrderWrite(SqlDbContext db) : IOrderWrite
{
    public async Task<int> CreateAsync(Order order, CancellationToken ct = default)
    {
        await using var tx = await db.Database.BeginTransactionAsync(ct);

        db.Orders.Add(order);
        await db.SaveChangesAsync(ct);

        await tx.CommitAsync(ct);

        return order.OrderId;
    }
}
