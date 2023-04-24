using LeokaEstetica.Platform.Models.Entities.Moderation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.Moderation;

public partial class RemarkStatuseConfiguration : IEntityTypeConfiguration<RemarkStatuseEntity>
{
    public void Configure(EntityTypeBuilder<RemarkStatuseEntity> entity)
    {
        entity.ToTable("RemarksStatuses", "Moderation");

        entity.HasKey(e => e.StatusId);

        entity.Property(e => e.StatusId)
            .HasColumnName("StatusId")
            .HasColumnType("serial")
            .IsRequired();

        entity.Property(e => e.StatusName)
            .HasColumnName("StatusName")
            .HasColumnType("varchar(150)")
            .HasMaxLength(150)
            .IsRequired();

        entity.Property(e => e.StatusSysName)
            .HasColumnName("StatusSysName")
            .HasColumnType("varchar(150)")
            .HasMaxLength(150)
            .IsRequired();

        entity.HasIndex(u => u.StatusId)
            .HasDatabaseName("PK_StatusId")
            .IsUnique();

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<RemarkStatuseEntity> entity);
}