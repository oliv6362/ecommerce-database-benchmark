using EcommerceDatabaseBenchmark.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EcommerceDatabaseBenchmark.Infrastructure.SqlServer.Seed;

/// <summary>
/// SQL Server seeder used to insert deterministic benchmark data into the relational schema
/// using Entity Framework Core.
///
/// Responsibilities:
/// 1) Seed Customers, Products, and Orders in an order that respects foreign key dependencies.
/// 2) Use IDENTITY_INSERT so predefined primary key values can be inserted, keeping ids stable
///    and comparable across database implementations in the benchmark.
/// 3) Wrap the seed operation in a transaction to ensure atomicity.
///
/// Seeding behavior:
/// - If the Customers table already contains any rows, seeding is skipped to avoid duplicates.
/// - Data is inserted in this order: Customers, then Products, then Orders.
/// - OrderItems are inserted when saving Orders via EF Core navigation relationships.
///
/// Clearing behavior:
/// - ClearAsync deletes data in reverse dependency order:
///   OrderItems, then Orders, then Products, then Customers.
/// </summary>
public sealed class SqlSeeder
{
    private readonly SqlDbContext _db;
    public SqlSeeder(SqlDbContext db) => _db = db;

    public async Task SeedAsync(IReadOnlyList<Customer> customers, IReadOnlyList<Product> products, IReadOnlyList<Order> orders, CancellationToken ct = default)
    {
        if (await _db.Customers.AnyAsync(ct))
            return;

        await using var tx = await _db.Database.BeginTransactionAsync(ct);

        // Customers
        await _db.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Customers ON;", ct);
        _db.Customers.AddRange(customers);
        await _db.SaveChangesAsync(ct);
        await _db.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Customers OFF;", ct);

        // Products
        await _db.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Products ON;", ct);
        _db.Products.AddRange(products);
        await _db.SaveChangesAsync(ct);
        await _db.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Products OFF;", ct);

        // Orders
        await _db.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Orders ON;", ct);
        _db.Orders.AddRange(orders);
        await _db.SaveChangesAsync(ct);
        await _db.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Orders OFF;", ct);

        await tx.CommitAsync(ct);
    }

    public async Task ClearAsync(CancellationToken ct = default)
    {
        await _db.Database.ExecuteSqlRawAsync("DELETE FROM OrderItems", ct);
        await _db.Database.ExecuteSqlRawAsync("DELETE FROM Orders", ct);
        await _db.Database.ExecuteSqlRawAsync("DELETE FROM Products", ct);
        await _db.Database.ExecuteSqlRawAsync("DELETE FROM Customers", ct);
    }
}
