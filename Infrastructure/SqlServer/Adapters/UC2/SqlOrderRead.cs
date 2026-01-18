using EcommerceDatabaseBenchmark.Application.Contracts.Dtos.UC2;
using EcommerceDatabaseBenchmark.Application.Interfaces.UC2;
using Microsoft.EntityFrameworkCore;

namespace EcommerceDatabaseBenchmark.Infrastructure.SqlServer.Adapters;

/// <summary>
/// SQL Server adapter responsible for retrieving order details by OrderId using Entity Framework Core.
/// </summary>
public sealed class SqlOrderRead(SqlDbContext db) : IOrderRead
{
    public async Task<OrderDetails?> GetDetailsAsync(int orderId, CancellationToken ct = default)
    {
        // Single query with joins, projected into DTO
        return await db.Orders
            .AsNoTracking()
            .Where(o => o.OrderId == orderId)
            .Select(o => new OrderDetails(
                o.OrderId,
                o.CustomerId,
                o.Status.ToString(),
                o.TotalAmount,
                o.CreatedAt,
                new CustomerSummary(
                    o.Customer.CustomerId,
                    o.Customer.FirstName,
                    o.Customer.LastName,
                    o.Customer.Email),
                o.OrderItems.Select(oi => new OrderItemDetails(
                    oi.ProductId,
                    oi.Product.Sku,
                    oi.Product.Name,
                    oi.Quantity,
                    oi.UnitPrice
                )).ToList()
            ))
            .SingleOrDefaultAsync(ct);
    }
}
