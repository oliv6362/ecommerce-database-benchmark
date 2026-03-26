using EcommerceDatabaseBenchmark.Application.Contracts.Dtos.UC1;
using EcommerceDatabaseBenchmark.Application.Interfaces.UC1;
using EcommerceDatabaseBenchmark.Domain.Entities;
using EcommerceDatabaseBenchmark.Domain.Enums;

namespace EcommerceDatabaseBenchmark.Application.Usecases.UC1;

/// <summary>
/// Use-case that is responsible for placing a new order in the system.
///
/// This use-case orchestrates the complete "place order" workflow by:
/// - Verifying that the customer exists
/// - Fetching product data required for pricing
/// - Creating an order aggregate with order items
/// - Calculating the total order amount
/// - Persisting the order via an abstract write port
/// 
/// Benchmark focus:
/// - Measures write performance for placing a new order with N items.
/// </summary>
public sealed class PlaceOrderUseCase
{
    private readonly ICustomerRead _customers;
    private readonly IProductRead _products;
    private readonly IOrderWrite _orders;

    public PlaceOrderUseCase(ICustomerRead customers, IProductRead products, IOrderWrite orders)
    {
        _customers = customers;
        _products = products;
        _orders = orders;
    }

    public async Task<int> ExecuteAsync(PlaceOrderRequest request, CancellationToken ct = default)
    {
        if (request.OrderItems.Count == 0)
            throw new InvalidOperationException("Order must contain at least one item.");

        if (request.OrderItems.Any(i => i.Quantity <= 0))
            throw new InvalidOperationException("Quantity must be greater than 0.");

        // 1) Validate customer exists
        var customerExists = await _customers.ExistsAsync(request.CustomerId, ct);
        if (!customerExists)
            throw new InvalidOperationException("Customer does not exist.");

        // 2) Fetch product snapshots 
        var productIds = request.OrderItems.Select(i => i.ProductId).Distinct().ToList();

        var productSnapshots = await _products.GetByIdsAsync(productIds, ct);

        // Ensure all requested products exist
        if (productSnapshots.Count != productIds.Count)
            throw new InvalidOperationException("One or more products do not exist.");

        // Build a lookup for fast mapping
        var priceByProductId = productSnapshots.ToDictionary(p => p.ProductId, p => p.Price);

        // 3) Create order + items
        var order = new Order
        {
            CustomerId = request.CustomerId,
            Status = OrderStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        foreach (var orderItem in request.OrderItems)
        {
            order.OrderItems.Add(new OrderItem
            {
                ProductId = orderItem.ProductId,
                Quantity = orderItem.Quantity,
                UnitPrice = priceByProductId[orderItem.ProductId]
            });
        }

        // 4) Calculate total
        order.TotalAmount = order.OrderItems.Sum(i => i.UnitPrice * i.Quantity);

        // 5) Persist
        return await _orders.CreateAsync(order, ct);
    }
}
