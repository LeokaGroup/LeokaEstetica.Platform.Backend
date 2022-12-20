using LeokaEstetica.Platform.Models.Entities.Moderation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.Moderation;

public partial class ModerationStatusConfiguration : IEntityTypeConfiguration<ModerationStatusEntity>
{
    public void Configure(EntityTypeBuilder<ModerationStatusEntity> entity)
    {
        entity.ToTable("ModerationStatuses", "Moderation");

        entity.HasKey(e => e.StatusId);

        entity.Property(e => e.StatusId)
            .HasColumnName("StatusId")
            .HasColumnType("serial");
        
        entity.Property(e => e.StatusName)
            .HasColumnName("StatusName")
            .HasColumnType("varchar(150)")
            .IsRequired();
        
        entity.Property(e => e.StatusSysName)
            .HasColumnName("StatusSysName")
            .HasColumnType("varchar(150)")
            .IsRequired();

        entity.HasIndex(u => u.StatusId)
            .HasDatabaseName("PK_ModerationStatuses_StatusId")
            .IsUnique();

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<ModerationStatusEntity> entity);
}