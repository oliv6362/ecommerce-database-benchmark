using EcommerceDatabaseBenchmark.Infrastructure.MongoDB.Seed;
using EcommerceDatabaseBenchmark.Infrastructure.SqlServer.Seed;
using EcommerceDatabaseBenchmark.Application.Interfaces.UC1;
using EcommerceDatabaseBenchmark.Application.Interfaces.UC2;
using EcommerceDatabaseBenchmark.Application.Interfaces.UC3;
using EcommerceDatabaseBenchmark.Application.Interfaces.UC4;
using EcommerceDatabaseBenchmark.Application.Usecases.UC1;
using EcommerceDatabaseBenchmark.Application.Usecases.UC2;
using EcommerceDatabaseBenchmark.Application.Usecases.UC3;
using EcommerceDatabaseBenchmark.Application.Usecases.UC4;
using EcommerceDatabaseBenchmark.Application.Seeding;
using EcommerceDatabaseBenchmark.Application.Benchmarking;
using EcommerceDatabaseBenchmark.Api.Contracts.Benchmark.Dtos;
using EcommerceDatabaseBenchmark.Application.Contracts.Benchmark.Dtos;
using EcommerceDatabaseBenchmark.Application.Contracts.Dtos.UC1;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceDatabaseBenchmark.Controllers;

/// <summary>
/// API controller that seeds benchmark datasets and runs repeatable benchmark executions
/// for UC1–UC4 against a selected provider (SQL Server or MongoDB).
/// </summary>
[ApiController]
[Route("benchmark")]
public sealed class BenchmarkController : ControllerBase
{
    /// <summary>
    /// Seeds the selected provider with a deterministic dataset based on a benchmark profile and seed.
    /// The same domain data is generated for both providers to keep benchmarks fair and comparable.
    /// </summary>
    /// 
    /// <example>
    /// POST /benchmark/seed?provider=sql&profile=medium&seed=42
    /// </example>
    [HttpPost("seed")]
    public async Task<ActionResult<SeedResponse>> SeedAsync(
        [FromQuery] string provider,
        [FromQuery] BenchmarkProfileName profile = BenchmarkProfileName.Small,
        [FromQuery] int seed = 42,
        CancellationToken ct = default)
    {
        provider = NormalizeProvider(provider);

        var p = BenchmarkProfiles.Get(profile);

        // 1) Generate deterministic domain data
        var gen = new SeedDataGenerator(seed);

        var customers = gen.GenerateCustomers(p.Customers);
        var products = gen.GenerateProducts(p.Products);
        var orders = gen.GenerateOrders(customers, products, p.Orders, p.MaxItemsPerOrder);

        // Find the customer that has the most orders (UC3)
        var heavyCustomerId = orders
            .GroupBy(o => o.CustomerId)
            .OrderByDescending(g => g.Count())
            .Select(g => g.Key)
            .First();

        // 2) Persist to the selected provider
        if (provider == "sql")
        {
            var sqlSeeder = HttpContext.RequestServices.GetRequiredService<SqlSeeder>();
            await sqlSeeder.ClearAsync(ct);
            await sqlSeeder.SeedAsync(customers, products, orders, ct);
        }
        else if (provider == "mongo")
        {
            var mongoSeeder = HttpContext.RequestServices.GetRequiredService<MongoSeeder>();
            await mongoSeeder.ClearAsync(ct);
            await mongoSeeder.SeedAsync(customers, products, orders, ct);
        }

        return Ok(new SeedResponse(
            Provider: provider,
            Profile: profile.ToString(),
            Seed: seed,
            Customers: p.Customers,
            Products: p.Products,
            Orders: p.Orders,
            MaxItemsPerOrder: p.MaxItemsPerOrder,
            SuggestedCustomerIdForUC3: heavyCustomerId,
            SuggestedOrderIdForUC2: 1 
        ));
    }

    /// <summary>
    /// Runs the benchmark suite (UC1–UC4) for the selected provider and returns aggregated timing statistics.
    /// 
    /// UC2/UC3/UC4 are executed first to keep the dataset stable (read benchmarks),
    /// followed by UC1 which performs writes and can mutate the dataset.
    /// </summary>
    /// 
    /// <example>
    /// POST /benchmark/run?provider=sql&iterations=30&customerId=195
    /// </example>
    [HttpPost("run")]
    public async Task<ActionResult<BenchmarkRunResponse>> RunAsync(
        [FromQuery] string provider,
        [FromQuery] int iterations = 20,
        [FromQuery] int customerId = 1, // UC3
        [FromQuery] int orderId = 1, // UC2
        [FromQuery] int pageSize = 20,
        [FromQuery] int page1 = 1,
        [FromQuery] int page10 = 10,
        [FromQuery] int lastDays = 30,
        [FromQuery] int topLimit = 10,
        CancellationToken ct = default)
    {
        provider = NormalizeProvider(provider);

        if (iterations <= 0 || iterations > 200)
            return BadRequest("iterations must be 1..200");

        // Build use cases using keyed ports for selected provider
        var (uc1, uc2, uc3, uc4) = CreateUseCases(provider);

        var runner = new BenchmarkRunner(uc1, uc2, uc3, uc4);

        // UC2/UC3/UC4 first (reads) - keep dataset stable before UC1 writes
        var uc2Res = await runner.BenchmarkUC2Async(orderId, iterations, ct);
        var uc3P1 = await runner.BenchmarkUC3Async(customerId, page1, pageSize, iterations, ct);
        var uc3P10 = await runner.BenchmarkUC3Async(customerId, page10, pageSize, iterations, ct);
        var uc4Res = await runner.BenchmarkUC4Async(lastDays, topLimit, iterations, ct);

        // UC1: test N=1,3,10 (write-heavy)
        var uc1_1 = await runner.BenchmarkUC1Async(
            "UC1.PlaceOrder.Items1",
            BuildPlaceOrderRequest(customerId: customerId, itemCount: 1),
            iterations, ct);

        var uc1_3 = await runner.BenchmarkUC1Async(
            "UC1.PlaceOrder.Items3",
            BuildPlaceOrderRequest(customerId: customerId, itemCount: 3),
            iterations, ct);

        var uc1_10 = await runner.BenchmarkUC1Async(
            "UC1.PlaceOrder.Items10",
            BuildPlaceOrderRequest(customerId: customerId, itemCount: 10),
            iterations, ct);

        return Ok(new BenchmarkRunResponse(
            Provider: provider,
            Iterations: iterations,
            Inputs: new BenchmarkInputs(
                OrderIdForUC2: orderId,
                CustomerIdForUC3: customerId,
                PageSize: pageSize,
                Page1: page1,
                Page10: page10,
                LastDaysForUC4: lastDays,
                TopLimitForUC4: topLimit),
            Results: new List<BenchmarkSummary>
            {
                uc2Res,
                uc3P1,
                uc3P10,
                uc4Res,
                uc1_1,
                uc1_3,
                uc1_10
            }
        ));
    }

    /// <summary>
    /// Normalizes and validates the provider query parameter.
    /// </summary>
    private static string NormalizeProvider(string provider)
    {
        provider = (provider ?? "").Trim().ToLowerInvariant();
        if (provider is not ("sql" or "mongo"))
            throw new InvalidOperationException("provider must be 'sql' or 'mongo'");
        return provider;
    }

    /// <summary>
    /// Creates the UC1–UC4 use case instances for a specific provider by resolving
    /// the provider-specific (keyed) ports from the DI container.
    /// </summary>
    private (PlaceOrderUseCase UC1, GetOrderDetailsUseCase UC2, GetCustomerOrderHistoryUseCase UC3, GetTopSellingProductsUseCase UC4) CreateUseCases(string provider)
    {
        provider = provider.ToLowerInvariant();
        var sp = HttpContext.RequestServices;

        return (
            new PlaceOrderUseCase(
                sp.GetRequiredKeyedService<ICustomerRead>(provider),
                sp.GetRequiredKeyedService<IProductRead>(provider),
                sp.GetRequiredKeyedService<IOrderWrite>(provider)
            ),
            new GetOrderDetailsUseCase(
                sp.GetRequiredKeyedService<IOrderRead>(provider)
            ),
            new GetCustomerOrderHistoryUseCase(
                sp.GetRequiredKeyedService<IOrderHistoryRead>(provider)
            ),
            new GetTopSellingProductsUseCase(
                sp.GetRequiredKeyedService<ITopProductsRead>(provider)
            )
        );
    }

    /// <summary>
    /// Builds a deterministic PlaceOrderRequest used for UC1 benchmarking.
    /// 
    /// The request uses predictable product ids (starting from 1) and a simple
    /// quantity pattern to ensure repeatable inputs across runs and providers.
    /// </summary>
    private static PlaceOrderRequest BuildPlaceOrderRequest(int customerId, int itemCount)
    {
        var items = Enumerable.Range(1, itemCount)
            .Select(i => new OrderItemRequest
            {
                ProductId = i, // ProductId 1..10
                Quantity = 1 + (i % 3)
            })
            .ToList();

        return new PlaceOrderRequest
        {
            CustomerId = customerId,
            OrderItems = items
        };
    }
}

