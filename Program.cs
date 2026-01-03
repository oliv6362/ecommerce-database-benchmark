using EcommerceDatabaseBenchmark.Application.Interfaces;
using EcommerceDatabaseBenchmark.Application.UseCases.PlaceOrder;
using EcommerceDatabaseBenchmark.Infrastructure.SqlServer;
using EcommerceDatabaseBenchmark.Infrastructure.SqlServer.Adapters;
using Microsoft.EntityFrameworkCore;
using EcommerceDatabaseBenchmark.Infrastructure.SqlServer.Seed;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<ICustomerRead, SqlCustomerRead>();
builder.Services.AddScoped<IProductRead, SqlProductRead>();
builder.Services.AddScoped<IOrderWrite, SqlOrderWrite>();

builder.Services.AddScoped<PlaceOrderUseCase>();
builder.Services.AddScoped<DatabaseSeeder>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// EF Core
builder.Services.AddDbContext<SqlDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("SqlServer")
    )
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
