// using LeokaEstetica.Platform.Models.Entities.Template;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.EntityFrameworkCore.Metadata.Builders;
//
// namespace LeokaEstetica.Platform.Models.Mappings.Template;
//
// // TODO: Проверить, похоже маппинг не правильно сделан многие-многие. Пока не через EF используем эту связь и выглядит не критично, но надо отрефачить и написать корректно многие-многие.
// public partial class ProjectManagmentTaskTemplateConfiguration : IEntityTypeConfiguration<ProjectManagmentTaskTemplateEntity>
// {
//     public void Configure(EntityTypeBuilder<ProjectManagmentTaskTemplateEntity> entity)
//     {
//         entity.ToTable("ProjectManagmentTaskTemplates", "Templates");
//
//         entity.HasKey(e => e.TemplateId);
//
//         entity.Property(e => e.TemplateId)
//             .HasColumnName("TemplateId")
//             .HasColumnType("serial");
//         
//         entity.Property(e => e.TemplateName)
//             .HasColumnName("TemplateName")
//             .HasColumnType("varchar(100)")
//             .HasMaxLength(100)
//             .IsRequired();
//         
//         entity.Property(e => e.TemplateSysName)
//             .HasColumnName("TemplateSysName")
//             .HasColumnType("varchar(100)")
//             .HasMaxLength(100)
//             .IsRequired();
//
//         entity.Property(e => e.Position)
//             .HasColumnName("Position")
//             .HasColumnType("int")
//             .IsRequired()
//             .HasDefaultValue(0);
//
//         entity.HasIndex(u => u.TemplateId)
//             .HasDatabaseName("PK_ProjectManagmentTaskTemplates_TemplateId")
//             .IsUnique();
//
//         // Для связей многие-ко-многим это обязательно.
//         entity.HasMany(c => c.ProjectManagmentTaskStatusTemplates)
//             .WithMany(s => s.ProjectManagmentTaskTemplates)
//             .UsingEntity<ProjectManagmentTaskStatusIntermediateTemplateEntity>(
//                 j => j
//                     .HasOne(pt => pt.ProjectManagmentTaskStatusTemplate)
//                     .WithMany(t => t.ProjectManagmentTaskStatusIntermediateTemplates)
//                     .HasForeignKey(pt => pt.StatusId),
//                 j => j
//                     .HasOne(pt => pt.ProjectManagmentTaskTemplate)
//                     .WithMany(p => p.ProjectManagmentTaskStatusIntermediateTemplates)
//                     .HasForeignKey(pt => pt.TemplateId),
//                 j =>
//                 {
//                     j.HasKey(t => new { t.TemplateId, t.StatusId });
//                     j.ToTable("ProjectManagmentTaskStatusIntermediateTemplates", "Templates");
//                 });
//
//         OnConfigurePartial(entity);
//     }
//
//     partial void OnConfigurePartial(EntityTypeBuilder<ProjectManagmentTaskTemplateEntity> entity);
// }