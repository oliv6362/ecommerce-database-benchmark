using EcommerceDatabaseBenchmark.Domain.Entities;
using EcommerceDatabaseBenchmark.Domain.Enums;

namespace EcommerceDatabaseBenchmark.Application.Seeding;

/// <summary>
/// Generates deterministic seed data for benchmarking SQL Server and MongoDB.
/// </summary>
public sealed class SeedDataGenerator
{
    private readonly Random _rng;
    private readonly DateTimeOffset _now;

    public SeedDataGenerator(int seed)
    {
        _rng = new Random(seed);
        _now = DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// Generates a list of customers with sequential ids and randomized creation dates.
    /// </summary>
    public IReadOnlyList<Customer> GenerateCustomers(int count)
    {
        return Enumerable.Range(1, count)
            .Select(i => new Customer
            {
                CustomerId = i,
                FirstName = $"First{i}",
                LastName = $"Last{i}",
                Email = $"customer{i}@benchmark.local",
                CreatedAt = _now.AddDays(-_rng.Next(30, 900))
            })
            .ToList();
    }

    /// <summary>
    /// Generates a list of products with sequential ids, formatted SKUs, random prices,
    /// and randomized creation dates.
    /// </summary>
    public IReadOnlyList<Product> GenerateProducts(int count)
    {
        return Enumerable.Range(1, count)
            .Select(i => new Product
            {
                ProductId = i,
                Sku = $"SKU-{i:D6}",
                Name = $"Product {i}",
                Price = GeneratePrice(),
                CreatedAt = _now.AddDays(-_rng.Next(30, 900))
            })
            .ToList();
    }

    /// <summary>
    /// Generates a random product price between 5.00 and 500.00.
    /// </summary>
    private decimal GeneratePrice()
    {
        return Math.Round((decimal)(_rng.NextDouble() * 495 + 5), 2);
    }

    /// <summary>
    /// Generates a list of orders using the provided customers and products, with a random
    /// number of items per order.
    /// </summary>
    public IReadOnlyList<Order> GenerateOrders(IReadOnlyList<Customer> customers, IReadOnlyList<Product> products, int orderCount, int maxItemsPerOrder = 10)
    {
        if (customers.Count == 0) throw new InvalidOperationException("Customers required");
        if (products.Count == 0) throw new InvalidOperationException("Products required");

        return Enumerable.Range(1, orderCount)
            .Select(i => GenerateOrder(i, customers, products, maxItemsPerOrder))
            .ToList();
    }

    /// <summary>
    /// Generates a single order with:
    /// - A customer chosen using a skewed distribution to simulate heavy and light buyers.
    /// - A random number of order items, limited by maxItemsPerOrder and available products.
    /// - Unique products per order by shuffling the product list before selection.
    /// - Random quantities per item between 1 and 5.
    /// - A creation date within the last year.
    /// - A calculated TotalAmount based on unit price and quantity.
    /// </summary>
    private Order GenerateOrder(int orderId, IReadOnlyList<Customer> customers, IReadOnlyList<Product> products, int maxItemsPerOrder)
    {
        var customer = PickCustomer(customers);

        var itemCount = Math.Min(_rng.Next(1, maxItemsPerOrder + 1), products.Count);

        var selectedProducts = products
            .OrderBy(_ => _rng.Next())
            .Take(itemCount);

        var items = selectedProducts
            .Select(p => new OrderItem
            {
                ProductId = p.ProductId,
                Quantity = _rng.Next(1, 6),
                UnitPrice = p.Price
            })
            .ToList();

        var createdAt = _now.AddDays(-_rng.Next(0, 365));

        return new Order
        {
            OrderId = orderId,
            CustomerId = customer.CustomerId,
            CreatedAt = createdAt,
            Status = OrderStatus.Shipped,
            OrderItems = items,
            TotalAmount = items.Sum(i => i.UnitPrice * i.Quantity)
        };
    }

    /// <summary>
    /// Picks a customer using a skewed distribution where a small group of customers
    /// receives most of the orders, to simulate realistic order history patterns. (UC3)
    /// </summary>
    private Customer PickCustomer(IReadOnlyList<Customer> customers)
    {
        // 20% of customers get 80% of orders
        if (_rng.NextDouble() < 0.8)
        {
            var heavyCount = Math.Max(1, customers.Count / 5);
            return customers[_rng.Next(heavyCount)];
        }

        return customers[_rng.Next(customers.Count)];
    }
}
