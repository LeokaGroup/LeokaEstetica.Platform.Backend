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
            .HasColumnType("bigserial");
        
        entity.Property(e => e.ProjectName)
            .HasColumnName("ProjectName")
            .HasColumnType("varchar(200)");
        
        entity.Property(e => e.UserId)
            .HasColumnName("UserId")
            .HasColumnType("bigint");
        
        entity.Property(e => e.ProjectDetails)
            .HasColumnName("ProjectDetails")
            .HasColumnType("text");
        
        entity.Property(e => e.ProjectIcon)
            .HasColumnName("ProjectIcon")
            .HasColumnType("text");
        
        entity.Property(e => e.ProjectCode)
            .HasColumnName("ProjectCode")
            .HasColumnType("uuid");
        
        entity.Property(e => e.DateCreated)
            .HasColumnName("DateCreated")
            .HasColumnType("timestamp");

        entity.HasIndex(u => u.ProjectId)
            .HasDatabaseName("PK_UserProjects_ProjectId")
            .IsUnique();

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<UserProjectEntity> entity);
}