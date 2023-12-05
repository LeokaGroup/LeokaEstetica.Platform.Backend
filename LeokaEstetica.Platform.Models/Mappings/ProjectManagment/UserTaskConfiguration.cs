using LeokaEstetica.Platform.Models.Entities.ProjectManagment;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.ProjectManagment;

public partial class UserTaskConfiguration : IEntityTypeConfiguration<UserTaskEntity>
{
    public void Configure(EntityTypeBuilder<UserTaskEntity> entity)
    {
        entity.ToTable("UserTasks", "ProjectManagment");

        entity.HasKey(e => e.TaskId);

        entity.Property(e => e.TaskId)
            .HasColumnName("TaskId")
            .HasColumnType("bigserial");
        
        entity.Property(e => e.TaskStatusId)
            .HasColumnName("TaskStatusId")
            .HasColumnType("int")
            .IsRequired();
        
        entity.Property(e => e.AuthorId)
            .HasColumnName("AuthorId")
            .HasColumnType("bigint")
            .IsRequired();

        entity.HasIndex(u => u.TaskId)
            .HasDatabaseName("PK_UserTasks_TaskId")
            .IsUnique();
        
        entity.HasOne(p => p.TaskResolution)
            .WithOne(b => b.UserTask)
            .HasConstraintName("FK_TaskResolutions_ResolutionId")
            .IsRequired();
        
        entity.HasOne(p => p.TaskStatus)
            .WithMany(b => b.UserTasks)
            .HasForeignKey(p => p.StatusId)
            .HasConstraintName("FK_TaskStatuses_StatusId")
            .IsRequired();
        
        entity.HasOne(p => p.UserProject)
            .WithOne(b => b.UserTask)
            .HasConstraintName("FK_UserProjects_ProjectId")
            .IsRequired();
        
        entity.HasOne(p => p.TaskType)
            .WithOne(b => b.UserTask)
            .HasConstraintName("FK_TaskTypes_TypeId")
            .IsRequired();

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<UserTaskEntity> entity);
}