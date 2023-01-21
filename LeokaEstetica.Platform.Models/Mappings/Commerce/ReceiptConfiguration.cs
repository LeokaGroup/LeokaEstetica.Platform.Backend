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
            .HasColumnType("bigserial");
        
        entity.Property(e => e.OrderId)
            .HasColumnName("OrderId")
            .HasColumnType("bigint");
        
        entity.Property(e => e.DateSend)
            .HasColumnName("DateSend")
            .HasColumnType("timestamp");
        
        entity.Property(e => e.DateCreated)
            .HasColumnName("DateCreated")
            .HasColumnType("timestamp");

        entity.HasIndex(u => u.ReceiptId)
            .HasDatabaseName("PK_Receipts_ReceiptId")
            .IsUnique();

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<ReceiptEntity> entity);
}