using LeokaEstetica.Platform.Models.Entities.Commerce;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.Commerce;

public partial class RefundConfiguration : IEntityTypeConfiguration<RefundEntity>
{
    public void Configure(EntityTypeBuilder<RefundEntity> entity)
    {
        entity.ToTable("Refunds", "Commerce");

        entity.HasKey(e => e.RefundId);

        entity.Property(e => e.RefundId)
            .HasColumnName("RefundId")
            .HasColumnType("bigserial");

        entity.Property(e => e.Price)
            .HasColumnName("Price")
            .HasColumnType("decimal(12,2)")
            .IsRequired();

        entity.Property(e => e.PaymentId)
            .HasColumnName("PaymentId")
            .HasColumnType("varchar(50)")
            .HasMaxLength(50)
            .IsRequired();
        
        entity.Property(e => e.DateCreated)
            .HasColumnName("DateCreated")
            .HasColumnType("timestamp")
            .HasDefaultValue(DateTime.Now)
            .IsRequired();
        
        entity.Property(e => e.Status)
            .HasColumnName("Status")
            .HasColumnType("varchar(50)")
            .IsRequired();
        
        entity.Property(e => e.RefundOrderId)
            .HasColumnName("RefundOrderId")
            .HasColumnType("varchar(50)")
            .IsRequired();

        entity.HasIndex(u => u.RefundId)
            .HasDatabaseName("PK_Refunds_RefundId")
            .IsUnique();

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<RefundEntity> entity);
}