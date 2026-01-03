using EcommerceDatabaseBenchmark.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;

namespace EcommerceDatabaseBenchmark.Infrastructure.SqlServer
{
    public class SqlDbContext : DbContext
    {
        public SqlDbContext(DbContextOptions<SqlDbContext> options) : base(options) { }

        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(SqlDbContext).Assembly);
        }
    }
}
