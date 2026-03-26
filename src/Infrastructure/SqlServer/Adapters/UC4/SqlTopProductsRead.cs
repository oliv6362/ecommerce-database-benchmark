using EcommerceDatabaseBenchmark.Application.Contracts.Dtos.UC4;
using EcommerceDatabaseBenchmark.Application.Interfaces.UC4;
using Microsoft.EntityFrameworkCore;

namespace EcommerceDatabaseBenchmark.Infrastructure.SqlServer.Adapters;

/// <summary>
/// SQL Server adapter responsible for retrieving top-selling products within a specified time range.
///
/// The query groups orderitems by product and sums the sold quantities to determine how many
/// units of each product have been sold. 
/// 
/// The aggregated result is then joined with the Products table for product information (e.g SKU, Name), 
/// and the final result is returned as a list of top-selling products.
/// </summary>
public sealed class SqlTopProductsRead(SqlDbContext db) : ITopProductsRead
{
    public async Task<TopProductsResult> GetTopSellingAsync(DateTimeOffset fromUtc, DateTimeOffset toUtc, int limit, CancellationToken ct = default)
    {
        var orderItems = await db.OrderItems
            .AsNoTracking()
            .Where(oi => oi.Order.CreatedAt >= fromUtc && oi.Order.CreatedAt < toUtc)
            .GroupBy(oi => oi.ProductId)
            .Select(g => new
            {
                ProductId = g.Key,
                QuantitySold = g.Sum(x => (long)x.Quantity)
            })
            .OrderByDescending(x => x.QuantitySold)
            .Take(limit)
            .Join(db.Products.AsNoTracking(),
                agg => agg.ProductId,
                p => p.ProductId,
                (agg, p) => new TopProductItem(
                    p.ProductId,
                    p.Sku,
                    p.Name,
                    agg.QuantitySold))
            .ToListAsync(ct);

        return new TopProductsResult(fromUtc, toUtc, limit, orderItems);
    }
}
