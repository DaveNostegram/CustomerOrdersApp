using CustomerOrdersApp.Domain.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CustomerOrdersApp.Infrastructure.Persistence.Configurations;

public class OrderItemConfig : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("OrderItems");
        builder.Property(x => x.PublicId)
            .HasDefaultValueSql("NEXT VALUE FOR OrderItemPublicIds")
            .IsRequired();
        builder.HasKey(x => x.Id);

        builder.Property(x => x.PublicId).IsRequired();
        builder.HasIndex(x => x.PublicId).IsUnique();

        builder.HasOne(x => x.Order)
            .WithMany(o => o.Items)
            .HasForeignKey(x => x.OrderId);

        builder.Property(x => x.ListPrice)
            .HasPrecision(18, 2)
            .IsRequired();
        builder.Property(x => x.FinalPrice)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.Discount)
            .HasPrecision(5, 4);
    }
}