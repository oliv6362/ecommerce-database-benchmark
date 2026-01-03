using EcommerceDatabaseBenchmark.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcommerceDatabaseBenchmark.Infrastructure.SqlServer.Configurations;

public class CustomerConfig : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> b)
    {
        b.ToTable("Customers");

        b.HasKey(x => x.CustomerId);

        b.Property(x => x.FirstName).HasMaxLength(100).IsRequired();
        b.Property(x => x.LastName).HasMaxLength(100).IsRequired();
        b.Property(x => x.Email).HasMaxLength(255).IsRequired();
        b.Property(x => x.CreatedAt).HasDefaultValueSql("SYSDATETIMEOFFSET()");

        // Indexes
        b.HasIndex(x => x.Email).IsUnique();
    }
}
