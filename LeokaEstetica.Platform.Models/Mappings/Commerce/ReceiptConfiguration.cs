using LeokaEstetica.Platform.Models.Entities.Commerce;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.Commerce;

public partial class ReceiptConfiguration : IEntityTypeConfiguration<ReceiptEntity>
{
    public void Configure(EntityTypeBuilder<ReceiptEntity> entity)
    {
        entity.ToTable("Receipts", "Commerce");

        entity.HasKey(e => e.ReceiptId);

        entity.Property(e => e.ReceiptId)
            .HasColumnName("ReceiptId")
            .HasColumnType("bigserial")
            .IsRequired();
        
        entity.Property(e => e.OrderId)
            .HasColumnName("OrderId")
            .HasColumnType("bigint")
            .IsRequired();

        entity.Property(e => e.DateCreated)
            .HasColumnName("DateCreated")
            .HasColumnType("timestamp")
            .IsRequired();
        
        entity.Property(e => e.Status)
            .HasColumnName("Status")
            .HasColumnType("varchar(50)")
            .IsRequired();
        
        entity.Property(e => e.Type)
            .HasColumnName("Type")
            .HasColumnType("varchar(50)")
            .IsRequired();
        
        entity.Property(e => e.ReceiptOrderId)
            .HasColumnName("ReceiptOrderId")
            .HasColumnType("varchar(50)")
            .IsRequired();
        
        entity.Property(e => e.UserId)
            .HasColumnName("UserId")
            .HasColumnType("bigint")
            .IsRequired();

        entity.HasIndex(u => u.ReceiptId)
            .HasDatabaseName("PK_Receipts_ReceiptId")
            .IsUnique();

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<ReceiptEntity> entity);
}