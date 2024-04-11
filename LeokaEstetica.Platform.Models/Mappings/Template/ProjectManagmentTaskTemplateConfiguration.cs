using LeokaEstetica.Platform.Models.Entities.Template;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.Template;

// TODO: Проверить, похоже маппинг не правильно сделан многие-многие. Пока не через EF используем эту связь и выглядит не критично, но надо отрефачить и написать корректно многие-многие.
public partial class ProjectManagmentTaskTemplateConfiguration : IEntityTypeConfiguration<ProjectManagmentTaskTemplateEntity>
{
    public void Configure(EntityTypeBuilder<ProjectManagmentTaskTemplateEntity> entity)
    {
        entity.ToTable("ProjectManagmentTaskTemplates", "Templates");

        entity.HasKey(e => e.TemplateId);

        entity.Property(e => e.TemplateId)
            .HasColumnName("TemplateId")
            .HasColumnType("serial");
        
        entity.Property(e => e.TemplateName)
            .HasColumnName("TemplateName")
            .HasColumnType("varchar(100)")
            .HasMaxLength(100)
            .IsRequired();
        
        entity.Property(e => e.TemplateSysName)
            .HasColumnName("TemplateSysName")
            .HasColumnType("varchar(100)")
            .HasMaxLength(100)
            .IsRequired();

        entity.Property(e => e.Position)
            .HasColumnName("Position")
            .HasColumnType("int")
            .IsRequired()
            .HasDefaultValue(0);

        entity.HasIndex(u => u.TemplateId)
            .HasDatabaseName("PK_ProjectManagmentTaskTemplates_TemplateId")
            .IsUnique();

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<ProjectManagmentTaskTemplateEntity> entity);
}