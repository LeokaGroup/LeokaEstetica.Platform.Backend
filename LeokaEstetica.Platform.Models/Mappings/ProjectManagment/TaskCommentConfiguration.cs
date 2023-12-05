using LeokaEstetica.Platform.Models.Entities.ProjectManagment;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.ProjectManagment;

public partial class TaskCommentConfiguration : IEntityTypeConfiguration<TaskCommentEntity>
{
    public void Configure(EntityTypeBuilder<TaskCommentEntity> entity)
    {
        entity.ToTable("TaskComments", "ProjectManagment");

        entity.HasKey(e => e.CommentId);

        entity.Property(e => e.CommentId)
            .HasColumnName("CommentId")
            .HasColumnType("bigserial");

        entity.Property(e => e.AuthorId)
            .HasColumnName("AuthorId")
            .HasColumnType("bigint")
            .IsRequired();
        
        entity.Property(e => e.Comment)
            .HasColumnName("Comment")
            .HasColumnType("text")
            .IsRequired();
        
        entity.Property(e => e.Created)
            .HasColumnName("Created")
            .HasColumnType("timestamp")
            .HasDefaultValue(DateTime.UtcNow)
            .IsRequired();
        
        entity.Property(e => e.Updated)
            .HasColumnName("Updated")
            .HasColumnType("timestamp")
            .HasDefaultValue(DateTime.UtcNow)
            .IsRequired();

        entity.HasIndex(u => u.CommentId)
            .HasDatabaseName("PK_TaskComments_CommentId")
            .IsUnique();

        entity.HasOne(p => p.UserTask)
            .WithMany(b => b.TaskComments)
            .HasForeignKey(p => p.TaskId)
            .HasConstraintName("FK_UserTasks_TaskId")
            .IsRequired();

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<TaskCommentEntity> entity);
}