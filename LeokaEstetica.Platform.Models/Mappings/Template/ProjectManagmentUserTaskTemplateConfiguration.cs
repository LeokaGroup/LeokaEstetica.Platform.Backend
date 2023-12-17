using LeokaEstetica.Platform.Models.Entities.Template;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.Template;

public partial class ProjectManagmentUserTaskTemplateConfiguration : IEntityTypeConfiguration<ProjectManagmentUserTaskTemplateEntity>
{
    public void Configure(EntityTypeBuilder<ProjectManagmentUserTaskTemplateEntity> entity)
    {
        entity.ToTable("ProjectManagmentUserTaskTemplates", "Templates");

        entity.HasKey(e => new { e.UserId, e.TemplateId });

        entity.Property(e => e.IsActive)
            .HasColumnName("IsActive")
            .HasColumnType("bool")
            .IsRequired();

        entity.HasIndex(u => u.UserId)
            .HasDatabaseName("FK_Users_UserId");

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<ProjectManagmentUserTaskTemplateEntity> entity);
}