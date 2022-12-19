using LeokaEstetica.Platform.Models.Entities.Project;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.Project;

public partial class ProjectCommentStatuseConfiguration : IEntityTypeConfiguration<ProjectCommentStatuseEntity>
{
    public void Configure(EntityTypeBuilder<ProjectCommentStatuseEntity> entity)
    {
        entity.ToTable("ProjectCommentsStatuses", "Projects");

        entity.HasKey(e => e.StatusId);

        entity.Property(e => e.StatusId)
            .HasColumnName("StatusId")
            .HasColumnType("serial");
        
        entity.Property(e => e.StatusName)
            .HasColumnName("StatusName")
            .HasColumnType("varchar(150)")
            .IsRequired();
        
        entity.Property(e => e.StatusSysName)
            .HasColumnName("StatusSysName")
            .HasColumnType("varchar(150)")
            .IsRequired();
        
        entity.HasOne(p => p.ProjectComment)
            .WithMany(b => b.ProjectCommentStatuses)
            .HasForeignKey(p => p.CommentId)
            .HasConstraintName("FK_ProjectComments_StatusId");

        entity.HasIndex(u => u.StatusId)
            .HasDatabaseName("PK_ProjectCommentsStatuses_StatusId")
            .IsUnique();

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<ProjectCommentStatuseEntity> entity);
}