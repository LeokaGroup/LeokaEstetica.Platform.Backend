using LeokaEstetica.Platform.Models.Entities.Landing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.Landing;

public partial class PlatformConditionConfiguration : IEntityTypeConfiguration<PlatformConditionEntity>
{
    public void Configure(EntityTypeBuilder<PlatformConditionEntity> entity)
    {
        entity.ToTable("PlatformConditions", "dbo");

        entity.HasKey(e => e.PlatformConditionId);

        entity.Property(e => e.PlatformConditionId)
            .HasColumnName("PlatformConditionId")
            .HasColumnType("serial");

        entity.Property(e => e.PlatformConditionTitle)
            .HasColumnName("PlatformConditionTitle")
            .HasColumnType("varchar(200)");

        entity.Property(e => e.PlatformConditionSubTitle)
            .HasColumnName("PlatformConditionSubTitle")
            .HasColumnType("varchar(200)")
            .IsRequired();

        entity.HasIndex(u => u.PlatformConditionId)
            .HasDatabaseName("PK_PlatformConditions_PlatformConditionId")
            .IsUnique();

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<PlatformConditionEntity> entity);
}