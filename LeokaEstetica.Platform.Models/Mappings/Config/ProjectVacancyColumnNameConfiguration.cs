using LeokaEstetica.Platform.Models.Entities.Configs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.Config;

public partial class ProjectVacancyColumnNameConfiguration : IEntityTypeConfiguration<ProjectVacancyColumnNameEntity>
{
    public void Configure(EntityTypeBuilder<ProjectVacancyColumnNameEntity> entity)
    {
        entity.ToTable("ProjectVacancyColumnsNames", "Configs");

        entity.HasKey(e => e.ColumnId);

        entity.Property(e => e.ColumnId)
            .HasColumnName("ColumnId")
            .HasColumnType("bigserial");

        entity.Property(e => e.ColumnName)
            .HasColumnName("ColumnName")
            .HasColumnType("varchar(200)")
            .IsRequired();

        entity.Property(e => e.TableName)
            .HasColumnName("TableName")
            .HasColumnType("varchar(200)")
            .IsRequired();

        entity.Property(e => e.Position)
            .HasColumnName("Position")
            .HasColumnType("int")
            .IsRequired();

        entity.HasIndex(u => u.ColumnId)
            .HasDatabaseName("PK_ProjectVacancyColumnsNames_ColumnId")
            .IsUnique();

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<ProjectVacancyColumnNameEntity> entity);
}