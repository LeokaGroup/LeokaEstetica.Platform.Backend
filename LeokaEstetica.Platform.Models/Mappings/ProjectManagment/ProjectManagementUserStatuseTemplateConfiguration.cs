using LeokaEstetica.Platform.Models.Entities.Template;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.ProjectManagment;

public partial class ProjectManagementUserStatuseTemplateConfiguration : IEntityTypeConfiguration<ProjectManagementUserStatuseTemplateEntity>
{
    public void Configure(EntityTypeBuilder<ProjectManagementUserStatuseTemplateEntity> entity)
    {
        entity.ToTable("ProjectManagementUserStatuseTemplates", "Templates");

        entity.HasKey(e => e.StatusId);

        entity.Property(e => e.StatusId)
            .HasColumnName("StatusId")
            .HasColumnType("bigserial");

        entity.Property(e => e.StatusName)
            .HasColumnName("StatusName")
            .HasColumnType("varchar(100)")
            .IsRequired();

        entity.Property(e => e.StatusSysName)
            .HasColumnName("StatusSysName")
            .HasColumnType("varchar(100)")
            .IsRequired();
        
        entity.Property(e => e.Position)
            .HasColumnName("Position")
            .HasColumnType("int")
            .IsRequired()
            .HasDefaultValue(0);
        
        entity.Property(e => e.UserId)
            .HasColumnName("UserId")
            .HasColumnType("bigint")
            .IsRequired();
        
        entity.Property(e => e.StatusDescription)
            .HasColumnName("StatusDescription")
            .HasColumnType("varchar(255)");

        entity.HasIndex(u => u.StatusId)
            .HasDatabaseName("PK_ProjectManagementUserStatuseTemplates_StatusId")
            .IsUnique();

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<ProjectManagementUserStatuseTemplateEntity> entity);
}