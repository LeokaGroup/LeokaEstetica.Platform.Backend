using LeokaEstetica.Platform.Models.Entities.Project;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.Project;

public partial class UserProjectConfiguration : IEntityTypeConfiguration<UserProjectEntity>
{
    public void Configure(EntityTypeBuilder<UserProjectEntity> entity)
    {
        entity.ToTable("UserProjects", "Projects");

        entity.HasKey(e => e.ProjectId);

        entity.Property(e => e.ProjectId)
            .HasColumnName("ProjectId")
            .HasColumnType("bigserial")
            .IsRequired();

        entity.Property(e => e.ProjectName)
            .HasColumnName("ProjectName")
            .HasColumnType("varchar(200)")
            .IsRequired();

        entity.Property(e => e.UserId)
            .HasColumnName("UserId")
            .HasColumnType("bigint")
            .IsRequired();

        entity.Property(e => e.ProjectDetails)
            .HasColumnName("ProjectDetails")
            .HasColumnType("text")
            .IsRequired();

        entity.Property(e => e.ProjectIcon)
            .HasColumnName("ProjectIcon")
            .HasColumnType("text");

        entity.Property(e => e.ProjectCode)
            .HasColumnName("ProjectCode")
            .HasColumnType("uuid")
            .HasMaxLength(36)
            .IsRequired();

        entity.Property(e => e.DateCreated)
            .HasColumnName("DateCreated")
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        entity.Property(e => e.PublicId)
            .HasColumnName("PublicId")
            .HasColumnType("uuid")
            .HasMaxLength(36)
            .IsRequired();

        entity.Property(e => e.Conditions)
            .HasColumnName("Conditions")
            .HasColumnType("text");
        
        entity.Property(e => e.Demands)
            .HasColumnName("Demands")
            .HasColumnType("text");
        
        entity.Property(e => e.TemplateId)
            .HasColumnName("TemplateId")
            .HasColumnType("int");

        entity.HasIndex(u => u.ProjectId)
            .HasDatabaseName("PK_UserProjects_ProjectId")
            .IsUnique();

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<UserProjectEntity> entity);
}