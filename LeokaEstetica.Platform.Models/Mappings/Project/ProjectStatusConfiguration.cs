using LeokaEstetica.Platform.Models.Entities.Project;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.Project;

public partial class ProjectStatusConfiguration : IEntityTypeConfiguration<ProjectStatusEntity>
{
    public void Configure(EntityTypeBuilder<ProjectStatusEntity> entity)
    {
        entity.ToTable("ProjectStatuses", "Projects");

        entity.HasKey(e => e.StatusId);

        entity.Property(e => e.StatusId)
            .HasColumnName("StatusId")
            .HasColumnType("serial");
        
        entity.Property(e => e.ProjectId)
            .HasColumnName("ProjectId")
            .HasColumnType("bigint");
        
        entity.Property(e => e.ProjectStatusSysName)
            .HasColumnName("ProjectStatusSysName")
            .HasColumnType("varchar(100)");
        
        entity.Property(e => e.ProjectStatusName)
            .HasColumnName("ProjectStatusName")
            .HasColumnType("varchar(100)");

        // entity.HasOne(p => p.ProjectId)
        //     .WithMany(b => b.ProjectId)
        //     .HasForeignKey(p => p.CatalogProjectId)
        //     .HasConstraintName("FK_UserProjects_ProjectId");

        entity.HasIndex(u => u.StatusId)
            .HasDatabaseName("PK_ProjectStatuses_StatusId")
            .IsUnique();

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<ProjectStatusEntity> entity);
}