using LeokaEstetica.Platform.Models.Entities.Template;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.Template;

public partial class ProjectManagmentTaskStatusTemplateConfiguration : IEntityTypeConfiguration<ProjectManagmentTaskStatusTemplateEntity>
{
    public void Configure(EntityTypeBuilder<ProjectManagmentTaskStatusTemplateEntity> entity)
    {
        entity.ToTable("ProjectManagmentTaskStatusTemplates", "Templates");

        entity.HasKey(e => e.StatusId);

        entity.Property(e => e.StatusId)
            .HasColumnName("StatusId")
            .HasColumnType("serial");

        entity.Property(e => e.StatusName)
            .HasColumnName("StatusName")
            .HasColumnType("varchar(100)")
            .HasMaxLength(100)
            .IsRequired();

        entity.Property(e => e.StatusSysName)
            .HasColumnName("StatusSysName")
            .HasColumnType("varchar(100)")
            .HasMaxLength(100)
            .IsRequired();

        entity.Property(e => e.Position)
            .HasColumnName("Position")
            .HasColumnType("int")
            .IsRequired()
            .HasDefaultValue(0);

        entity.HasIndex(u => u.StatusId)
            .HasDatabaseName("PK_ProjectManagmentTaskStatusTemplates_StatusId")
            .IsUnique();

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<ProjectManagmentTaskStatusTemplateEntity> entity);
}