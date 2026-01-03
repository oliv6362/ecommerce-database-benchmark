using EcommerceDatabaseBenchmark.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;

namespace EcommerceDatabaseBenchmark.Infrastructure.SqlServer.Ports;

/// <summary>
/// SQL Server adapter responsible for retrieving product data (Ids & price) using Entity Framework Core.
/// </summary>
public sealed class SqlProductRead(SqlDbContext db) : IProductRead
{
    public async Task<IReadOnlyList<ProductSnapshot>> GetByIdsAsync(IReadOnlyList<int> productIds, CancellationToken ct = default)
    {
        return await db.Products
            .AsNoTracking()
            .Where(p => productIds.Contains(p.ProductId))
            .Select(p => new ProductSnapshot(p.ProductId, p.Price))
            .ToListAsync(ct);
    }
}
