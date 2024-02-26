using LeokaEstetica.Platform.Models.Entities.Template;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.Template;

// TODO: Проверить, похоже маппинг не правильно сделан многие-многие. Пока не через EF используем эту связь и выглядит не критично, но надо отрефачить и написать корректно многие-многие.
public partial class ProjectManagmentTaskStatusTemplatesConfiguration : IEntityTypeConfiguration<ProjectManagmentTaskStatusIntermediateTemplateEntity>
{
    public void Configure(EntityTypeBuilder<ProjectManagmentTaskStatusIntermediateTemplateEntity> entity)
    {
        entity.ToTable("ProjectManagmentTaskStatusIntermediateTemplates", "Templates");

        entity.HasKey(e => new { e.StatusId, e.TemplateId });

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<ProjectManagmentTaskStatusIntermediateTemplateEntity> entity);
}