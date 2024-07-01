using LeokaEstetica.Platform.Models.Entities.Moderation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.Moderation;

public partial class ProjectCommentModerationConfiguration : IEntityTypeConfiguration<ProjectCommentModerationEntity>
{
    public void Configure(EntityTypeBuilder<ProjectCommentModerationEntity> entity)
    {
        entity.ToTable("ProjectCommentsModeration", "Moderation");

        entity.HasKey(e => e.ModerationId);

        entity.Property(e => e.ModerationId)
            .HasColumnName("ModerationId")
            .HasColumnType("bigserial");
        
        entity.Property(e => e.DateModeration)
            .HasColumnName("DateModeration")
            .HasColumnType("timestamp with time zone")
            .IsRequired();
        
        entity.Property(e => e.CommentId)
            .HasColumnName("CommentId")
            .HasColumnType("bigint")
            .IsRequired();
        
        entity.Property(e => e.ModerationStatusId)
            .HasColumnName("ModerationStatusId")
            .HasColumnType("int")
            .IsRequired();
        
        entity.HasOne(p => p.ModerationStatuses)
            .WithMany(b => b.ProjectCommentsModeration)
            .HasForeignKey(p => p.ModerationStatusId)
            .HasConstraintName("FK_ProjectComments_StatusId");
        
        entity.HasOne(p => p.ProjectComment)
            .WithMany(b => b.ProjectCommentsModeration)
            .HasForeignKey(p => p.CommentId)
            .HasConstraintName("FK_ModerationStatuses_CommentId");

        entity.HasIndex(u => u.ModerationId)
            .HasDatabaseName("PK_ProjectCommentsModeration_ModerationId")
            .IsUnique();

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<ProjectCommentModerationEntity> entity);
}