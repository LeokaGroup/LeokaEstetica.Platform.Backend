using LeokaEstetica.Platform.Models.Entities.Commerce;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.Commerce;

public partial class OrderConfiguration : IEntityTypeConfiguration<OrderEntity>
{
    public void Configure(EntityTypeBuilder<OrderEntity> entity)
    {
        entity.ToTable("Orders", "Commerce");

        entity.HasKey(e => e.OrderId);

        entity.Property(e => e.OrderId)
            .HasColumnName("OrderId")
            .HasColumnType("bigserial");
        
        entity.Property(e => e.OrderName)
            .HasColumnName("OrderName")
            .HasColumnType("varchar(200)");
        
        entity.Property(e => e.OrderDetails)
            .HasColumnName("OrderDetails")
            .HasColumnType("varchar(300)");
        
        entity.Property(e => e.DateCreated)
            .HasColumnName("DateCreated")
            .HasColumnType("timestamp");
        
        entity.Property(e => e.UserId)
            .HasColumnName("UserId")
            .HasColumnType("bigint");
        
        entity.Property(e => e.StatusName)
            .HasColumnName("StatusName")
            .HasColumnType("varchar(50)")
            .HasMaxLength(50);
        
        entity.Property(e => e.StatusSysName)
            .HasColumnName("StatusSysName")
            .HasColumnType("varchar(50)")
            .HasMaxLength(50);
        
        entity.Property(e => e.Price)
            .HasColumnName("Price")
            .HasColumnType("decimal(12,2)");
        
        entity.Property(e => e.PaymentMonth)
            .HasColumnName("PaymentMonth")
            .HasColumnType("smallint");
        
        entity.Property(e => e.Currency)
            .HasColumnName("Currency")
            .HasColumnType("varchar(5)")
            .HasMaxLength(5);

        entity.HasIndex(u => u.OrderId)
            .HasDatabaseName("PK_Orders_OrderId")
            .IsUnique();

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<OrderEntity> entity);
}