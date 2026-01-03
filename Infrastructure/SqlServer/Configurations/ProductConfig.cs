using EcommerceDatabaseBenchmark.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcommerceDatabaseBenchmark.Infrastructure.SqlServer.Configurations;

public class ProductConfig : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> b)
    {
        b.ToTable("Products");

        b.HasKey(x => x.ProductId);

        b.Property(x => x.Sku).HasMaxLength(64).IsRequired();
        b.Property(x => x.Name).HasMaxLength(200).IsRequired();
        b.Property(x => x.Price).HasPrecision(18, 2).IsRequired();
        b.Property(x => x.CreatedAt).HasDefaultValueSql("SYSDATETIMEOFFSET()");

        b.HasIndex(x => x.Sku).IsUnique();

        // Constraints
        b.ToTable(t =>
        {
            t.HasCheckConstraint("CK_Products_Price_NonNegative", "[Price] >= 0");
        });
    }
}
