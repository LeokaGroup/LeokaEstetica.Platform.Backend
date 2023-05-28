using LeokaEstetica.Platform.Models.Entities.Configs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.Config;

public partial class GlobalConfigConfiguration : IEntityTypeConfiguration<GlobalConfigEntity>
{
    public void Configure(EntityTypeBuilder<GlobalConfigEntity> entity)
    {
        entity.ToTable("GlobalConfig", "Configs");

        entity.HasKey(e => e.ParamId);

        entity.Property(e => e.ParamId)
            .HasColumnName("ParamId")
            .HasColumnType("bigserial");

        entity.Property(e => e.ParamKey)
            .HasColumnName("ParamKey")
            .HasColumnType("varchar(200)")
            .IsRequired();

        entity.Property(e => e.ParamValue)
            .HasColumnName("ParamValue")
            .HasColumnType("varchar(50)")
            .IsRequired();

        entity.Property(e => e.ParamType)
            .HasColumnName("ParamType")
            .HasColumnType("varchar(50)")
            .IsRequired();
        
        entity.Property(e => e.ParamDescription)
            .HasColumnName("ParamDescription")
            .HasColumnType("varchar(200)")
            .IsRequired();
        
        entity.Property(e => e.ParamTag)
            .HasColumnName("ParamTag")
            .HasColumnType("varchar(50)");

        entity.HasIndex(u => u.ParamId)
            .HasDatabaseName("PK_GlobalConfig_ParamId")
            .IsUnique();

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<GlobalConfigEntity> entity);
}