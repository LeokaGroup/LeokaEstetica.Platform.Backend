using LeokaEstetica.Platform.Models.Entities.Configs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.Config;

public partial class ValidationColumnExcludeConfiguration : IEntityTypeConfiguration<ValidationColumnExcludeEntity>
{
    public void Configure(EntityTypeBuilder<ValidationColumnExcludeEntity> entity)
    {
        entity.ToTable("ValidationColumnsExclude", "Configs");

        entity.HasKey(e => e.ValidationId);

        entity.Property(e => e.ValidationId)
            .HasColumnName("ValidationId")
            .HasColumnType("serial");

        entity.Property(e => e.ParamName)
            .HasColumnName("ParamName")
            .HasColumnType("varchar(150)")
            .IsRequired();

        entity.HasIndex(u => u.ValidationId)
            .HasDatabaseName("PK_ValidationColumnsExclude_ValidationId")
            .IsUnique();

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<ValidationColumnExcludeEntity> entity);
}