using LeokaEstetica.Platform.Models.Entities.Communication;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.Communication;

public partial class ProjectCommentConfiguration : IEntityTypeConfiguration<ProjectCommentEntity>
{
    public void Configure(EntityTypeBuilder<ProjectCommentEntity> entity)
    {
        entity.ToTable("ProjectComments", "Communications");

        entity.HasKey(e => e.CommentId);

        entity.Property(e => e.CommentId)
            .HasColumnName("CommentId")
            .HasColumnType("bigserial");
        
        entity.Property(e => e.ProjectId)
            .HasColumnName("ProjectId")
            .HasColumnType("bigint")
            .IsRequired();
        
        entity.Property(e => e.Created)
            .HasColumnName("Created")
            .HasColumnType("timestamp with time zone")
            .IsRequired();
        
        entity.Property(e => e.Comment)
            .HasColumnName("Comment")
            .HasColumnType("text")
            .IsRequired();
        
        entity.Property(e => e.IsMyComment)
            .HasColumnName("IsMyComment")
            .HasColumnType("bool")
            .IsRequired();
        
        entity.HasOne(p => p.User)
            .WithMany(b => b.ProjectComments)
            .HasForeignKey(p => p.UserId)
            .HasConstraintName("FK_Users_UserId");

        entity.HasIndex(u => u.CommentId)
            .HasDatabaseName("PK_ProjectComments_CommentId")
            .IsUnique();
        
        entity.HasOne(p => p.ModerationStatus)
            .WithMany(b => b.ProjectComments)
            .HasForeignKey(p => p.ModerationStatusId)
            .HasConstraintName("FK_ModerationStatuses_StatusId");

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<ProjectCommentEntity> entity);
}