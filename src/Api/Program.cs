using EcommerceDatabaseBenchmark.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services
    .AddSqlServerInfrastructure(builder.Configuration)
    .AddMongoInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseAuthorization();
app.MapControllers();

await app.EnsureMongoIndexesAsync();

app.Run();
