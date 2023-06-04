using LeokaEstetica.Platform.Models.Entities.Commerce;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.Commerce;

public partial class HistoryConfiguration : IEntityTypeConfiguration<HistoryEntity>
{
    public void Configure(EntityTypeBuilder<HistoryEntity> entity)
    {
        entity.ToTable("OrderTransactionsShadow", "Commerce");

        entity.HasKey(e => e.ShadowId);
        
        entity.Property(e => e.ShadowId)
            .HasColumnName("ShadowId")
            .HasColumnType("bigserial");

        entity.Property(e => e.OrderId)
            .HasColumnName("OrderId")
            .HasColumnType("bigserial");
        
        entity.Property(e => e.ActionText)
            .HasColumnName("ActionText")
            .HasColumnType("varchar(300)")
            .HasMaxLength(300);
        
        entity.Property(e => e.ActionSysName)
            .HasColumnName("ActionSysName")
            .HasColumnType("varchar(100)")
            .HasMaxLength(100);
        
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

        entity.HasIndex(u => u.ShadowId)
            .HasDatabaseName("PK_OrderTransactionsShadow_ShadowId")
            .IsUnique();

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<HistoryEntity> entity);
}