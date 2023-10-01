using LeokaEstetica.Platform.Models.Entities.Project;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.Project;

public partial class ProjectResponseStatuseConfiguration : IEntityTypeConfiguration<ProjectResponseStatuseEntity>
{
    public void Configure(EntityTypeBuilder<ProjectResponseStatuseEntity> entity)
    {
        entity.ToTable("ProjectResponseStatuses", "Projects");

        entity.HasKey(e => e.StatusId);

        entity.Property(e => e.StatusId)
            .HasColumnName("StatusId")
            .HasColumnType("serial")
            .IsRequired();

        entity.Property(e => e.StatusName)
            .HasColumnName("StatusName")
            .HasColumnType("varchar(150)")
            .IsRequired();

        entity.Property(e => e.StatusSysName)
            .HasColumnName("StatusSysName")
            .HasColumnType("varchar(150)")
            .IsRequired();

        entity.HasIndex(u => u.StatusId)
            .HasDatabaseName("PK_ProjectResponseStatuses_StatusId")
            .IsUnique();

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<ProjectResponseStatuseEntity> entity);
}