using LeokaEstetica.Platform.Models.Entities.Configs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.Config;

public partial class ConfigSpaceSettingConfiguration : IEntityTypeConfiguration<ConfigSpaceSettingEntity>
{
    public void Configure(EntityTypeBuilder<ConfigSpaceSettingEntity> entity)
    {
        entity.ToTable("ProjectManagmentProjectSettings", "Configs");

        entity.HasKey(e => e.ConfigId);

        entity.Property(e => e.ConfigId)
            .HasColumnName("ConfigId")
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
        
        entity.Property(e => e.ProjectId)
            .HasColumnName("ProjectId")
            .HasColumnType("bigint")
            .IsRequired();
        
        entity.Property(e => e.UserId)
            .HasColumnName("UserId")
            .HasColumnType("bigint")
            .IsRequired();
        
        entity.Property(e => e.LastUserDate)
            .HasColumnName("LastUserDate")
            .HasColumnType("timestamp with time zone")
            .HasDefaultValue(DateTime.UtcNow)
            .IsRequired();

        entity.HasIndex(u => u.ConfigId)
            .HasDatabaseName("PK_ProjectManagmentProjectSettings_ConfigId")
            .IsUnique();

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<ConfigSpaceSettingEntity> entity);
}