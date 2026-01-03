using EcommerceDatabaseBenchmark.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcommerceDatabaseBenchmark.Infrastructure.SqlServer.Configurations;

public class OrderItemConfig : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> b)
    {
        b.ToTable("OrderItems");

        b.HasKey(x => x.OrderItemId);

        b.Property(x => x.Quantity).IsRequired();
        b.Property(x => x.UnitPrice).HasPrecision(18, 2).IsRequired();

        // Relationships
        b.HasOne(x => x.Order)
            .WithMany(x => x.OrderItems)
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(x => x.Product)
            .WithMany(x => x.OrderItems)
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        b.HasIndex(x => new { x.OrderId, x.ProductId }).IsUnique();
        b.HasIndex(x => x.ProductId);


        // Constraints
        b.ToTable(t =>
        {
            t.HasCheckConstraint("CK_OrderItems_Quantity_Positive", "[Quantity] > 0");
            t.HasCheckConstraint("CK_OrderItems_UnitPrice_NonNegative", "[UnitPrice] >= 0");
        });
    }
}
