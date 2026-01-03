using EcommerceDatabaseBenchmark.Application.Dtos.UC3;
using EcommerceDatabaseBenchmark.Application.Interfaces.UC3;
using Microsoft.EntityFrameworkCore;

namespace EcommerceDatabaseBenchmark.Infrastructure.SqlServer.Adapters;


/// <summary>
/// SQL Server adapter responsible for retrieving a paged order history for a specific customer.
///
/// This adapter implements offset-based paging using Skip/Take and returns the customer's
/// most recent orders sorted by creation time. (e.g. page 1 vs page 10).
/// </summary>
public sealed class SqlOrderHistoryRead(SqlDbContext db) : IOrderHistoryRead
{
    public async Task<OrderHistoryPage> GetPageAsync(int customerId, int pageNumber, int pageSize, CancellationToken ct = default)
    {
        //how many rows that the database skips
        var skip = (pageNumber - 1) * pageSize;

        var orders = await db.Orders
            .AsNoTracking()
            .Where(o => o.CustomerId == customerId)
            // stable ordering for paging
            .OrderByDescending(o => o.CreatedAt)
            .ThenByDescending(o => o.OrderId)
            .Skip(skip)
            .Take(pageSize)
            .Select(o => new OrderHistoryItem(
                o.OrderId,
                o.CreatedAt,
                o.Status.ToString(),
                o.TotalAmount))
            .ToListAsync(ct);

        return new OrderHistoryPage(customerId, pageNumber, pageSize, orders);
    }
}
