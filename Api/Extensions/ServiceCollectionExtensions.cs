using EcommerceDatabaseBenchmark.Application.Interfaces.UC1;
using EcommerceDatabaseBenchmark.Application.Interfaces.UC2;
using EcommerceDatabaseBenchmark.Application.Interfaces.UC3;
using EcommerceDatabaseBenchmark.Application.Interfaces.UC4;
using EcommerceDatabaseBenchmark.Infrastructure.MongoDB;
using EcommerceDatabaseBenchmark.Infrastructure.MongoDB.Adapters.UC1;
using EcommerceDatabaseBenchmark.Infrastructure.MongoDB.Adapters.UC2;
using EcommerceDatabaseBenchmark.Infrastructure.MongoDB.Adapters.UC3;
using EcommerceDatabaseBenchmark.Infrastructure.MongoDB.Adapters.UC4;
using EcommerceDatabaseBenchmark.Infrastructure.MongoDB.Indexes;
using EcommerceDatabaseBenchmark.Infrastructure.MongoDB.Seed;
using EcommerceDatabaseBenchmark.Infrastructure.SqlServer;
using EcommerceDatabaseBenchmark.Infrastructure.SqlServer.Adapters;
using EcommerceDatabaseBenchmark.Infrastructure.SqlServer.Adapters.UC1;
using EcommerceDatabaseBenchmark.Infrastructure.SqlServer.Seed;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace EcommerceDatabaseBenchmark.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers all SQL Server–based infrastructure services, including:
    /// - Keyed ports & adapters for UC1–UC4 using SQL Server implementations.
    /// - The SQL Server seeder.
    /// - The SqlDbContext configured with a connection string from configuration
    ///   and a password read from the SA_PASSWORD environment variable.
    /// </summary>
    public static IServiceCollection AddSqlServerInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        // Ports & Adapters (SQL Server)
        services.AddKeyedScoped<ICustomerRead, SqlCustomerRead>("sql");
        services.AddKeyedScoped<IProductRead, SqlProductRead>("sql");
        services.AddKeyedScoped<IOrderWrite, SqlOrderWrite>("sql");
        services.AddKeyedScoped<IOrderRead, SqlOrderRead>("sql");
        services.AddKeyedScoped<ITopProductsRead, SqlTopProductsRead>("sql");
        services.AddKeyedScoped<IOrderHistoryRead, SqlOrderHistoryRead>("sql");

        services.AddScoped<SqlSeeder>();

        // DbContext with password from env
        var sqlBase = config.GetConnectionString("SqlServer")
            ?? throw new InvalidOperationException("Missing ConnectionStrings:SqlServer");

        var sqlPassword = config["SA_PASSWORD"]
            ?? throw new InvalidOperationException("Missing environment variable: SA_PASSWORD");

        var csb = new SqlConnectionStringBuilder(sqlBase)
        {
            Password = sqlPassword
        };

        services.AddDbContext<SqlDbContext>(options =>
            options.UseSqlServer(csb.ConnectionString));

        return services;
    }

    /// <summary>
    /// Registers all MongoDB-based infrastructure services, including:
    /// - Binding MongoOptions from configuration.
    /// - Injecting Mongo credentials from user-secrets into the connection string.
    /// - Keyed ports & adapters for UC1–UC4 using MongoDB implementations.
    /// - Core MongoDB services (MongoDb, index initializer, id generator, seeder).
    /// </summary>
    public static IServiceCollection AddMongoInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        // Bind options
        services.Configure<MongoOptions>(config.GetSection("Mongo"));

        // Credentials
        var mongoUser = config["MONGO_USERNAME"];
        var mongoPassword = config["MONGO_PASSWORD"];

        if (string.IsNullOrWhiteSpace(mongoUser))
            throw new InvalidOperationException("Missing MONGO_USERNAME");

        if (string.IsNullOrWhiteSpace(mongoPassword))
            throw new InvalidOperationException("Missing MONGO_PASSWORD");

        services.PostConfigure<MongoOptions>(opt =>
        {
            var b = new MongoUrlBuilder(opt.ConnectionString)
            {
                Username = mongoUser,
                Password = mongoPassword
            };
            opt.ConnectionString = b.ToString();
        });

        // Keyed ports & adapters (MongoDB)
        services.AddKeyedScoped<ICustomerRead, MongoCustomerRead>("mongo");
        services.AddKeyedScoped<IProductRead, MongoProductRead>("mongo");
        services.AddKeyedScoped<IOrderWrite, MongoOrderWrite>("mongo");
        services.AddKeyedScoped<IOrderRead, MongoOrderRead>("mongo");
        services.AddKeyedScoped<ITopProductsRead, MongoTopProductsRead>("mongo");
        services.AddKeyedScoped<IOrderHistoryRead, MongoOrderHistoryRead>("mongo");

        services.AddSingleton<MongoDb>();
        services.AddSingleton<MongoIndexInitializer>();
        services.AddSingleton<MongoIdGenerator>();
        services.AddScoped<MongoSeeder>();

        return services;
    }
}
