using LeokaEstetica.Platform.Models.Entities.Project;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.Project;

public partial class ProjectConfiguration : IEntityTypeConfiguration<ProjectEntity>
{
    public void Configure(EntityTypeBuilder<ProjectEntity> entity)
    {
        entity.ToTable("CatalogProjects", "Projects");

        entity.HasKey(e => e.ProjectId);

        entity.Property(e => e.ProjectId)
            .HasColumnName("ProjectId")
            .HasColumnType("bigserial");

        entity.Property(e => e.ProjectName)
            .HasColumnName("ProjectName")
            .HasColumnType("varchar(200)")
            .IsRequired();

        entity.Property(e => e.ProjectDetails)
            .HasColumnName("ProjectDetails")
            .HasColumnType("text")
            .IsRequired();
        
        entity.Property(e => e.ProjectIcon)
            .HasColumnName("ProjectIcon")
            .HasColumnType("text");
        
        entity.Property(e => e.UserId)
            .HasColumnName("UserId")
            .HasColumnType("bigint");
        
        entity.Property(e => e.ProjectCode)
            .HasColumnName("ProjectCode")
            .HasColumnType("uuid");

        entity.HasIndex(u => u.ProjectId)
            .HasDatabaseName("PK_CatalogProjects_ProjectId")
            .IsUnique();

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<ProjectEntity> entity);
}