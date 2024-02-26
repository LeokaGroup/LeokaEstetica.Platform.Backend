using LeokaEstetica.Platform.Models.Entities.Template;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.Template;

public partial class ProjectManagementTransitionTemplateConfiguration : IEntityTypeConfiguration<ProjectManagementTransitionTemplateEntity>
{
    public void Configure(EntityTypeBuilder<ProjectManagementTransitionTemplateEntity> entity)
    {
        entity.ToTable("ProjectManagementTransitionTemplates", "Templates");

        entity.HasKey(e => e.TransitionId);

        entity.Property(e => e.TransitionId)
            .HasColumnName("TransitionId")
            .HasColumnType("bigserial");

        entity.Property(e => e.TransitionName)
            .HasColumnName("TransitionName")
            .HasColumnType("varchar(150)")
            .IsRequired();

        entity.Property(e => e.TransitionSysName)
            .HasColumnName("TransitionSysName")
            .HasColumnType("varchar(150)")
            .IsRequired();
        
        entity.Property(e => e.Position)
            .HasColumnName("Position")
            .HasColumnType("int")
            .IsRequired()
            .HasDefaultValue(0);
        
        entity.Property(e => e.FromStatusId)
            .HasColumnName("FromStatusId")
            .HasColumnType("bigint")
            .IsRequired();
        
        entity.Property(e => e.ToStatusId)
            .HasColumnName("ToStatusId")
            .HasColumnType("bigint")
            .IsRequired();

        entity.HasIndex(u => u.TransitionId)
            .HasDatabaseName("PK_ProjectManagementTransitionTemplates_TransitionId")
            .IsUnique();

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<ProjectManagementTransitionTemplateEntity> entity);
}