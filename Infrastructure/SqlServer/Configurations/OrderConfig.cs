using EcommerceDatabaseBenchmark.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcommerceDatabaseBenchmark.Infrastructure.SqlServer.Configurations;

public class OrderConfig : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> b)
    {
        b.ToTable("Orders");

        b.HasKey(x => x.OrderId);

        b.Property(x => x.Status).HasConversion<int>().IsRequired();
        b.Property(x => x.TotalAmount).HasPrecision(18, 2).IsRequired();
        b.Property(x => x.CreatedAt).HasDefaultValueSql("SYSDATETIMEOFFSET()").IsRequired(); ;

        // Relationships
        b.HasOne(x => x.Customer)
            .WithMany(x => x.Orders)
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasMany(x => x.OrderItems)
            .WithOne(x => x.Order)
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        b.HasIndex(x => new { x.CustomerId, x.CreatedAt, x.OrderId });
        b.HasIndex(x => x.CreatedAt); 

        // Constraints
        b.ToTable(t =>
        {
            t.HasCheckConstraint("CK_Orders_TotalAmount_NonNegative", "[TotalAmount] >= 0");
            t.HasCheckConstraint("CK_Orders_Status_Valid", "[Status] IN (0,1,2)");

        });
    }
}
