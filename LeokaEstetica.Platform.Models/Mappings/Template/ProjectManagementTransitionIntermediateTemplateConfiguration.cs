using LeokaEstetica.Platform.Models.Entities.Template;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.Template;

public partial class ProjectManagementTransitionIntermediateTemplateConfiguration : IEntityTypeConfiguration<ProjectManagementTransitionIntermediateTemplateEntity>
{
    public void Configure(EntityTypeBuilder<ProjectManagementTransitionIntermediateTemplateEntity> entity)
    {
        entity.ToTable("ProjectManagementTransitionIntermediateTemplates", "Templates");

        entity.HasKey(e => new { e.TransitionId, e.FromStatusId, e.ToStatusId });

        entity.Property(e => e.IsCustomTransition)
            .HasColumnName("IsCustomTransition")
            .HasColumnType("boolean")
            .IsRequired();

        entity.HasIndex(e => new { e.TransitionId, e.FromStatusId, e.ToStatusId })
            .HasDatabaseName("PK_ProjectManagementTransitionIntermediateTemplates_TransitionI")
            .IsUnique();

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<ProjectManagementTransitionIntermediateTemplateEntity> entity);
}