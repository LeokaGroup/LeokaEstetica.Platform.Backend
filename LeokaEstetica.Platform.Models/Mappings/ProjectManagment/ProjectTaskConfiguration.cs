using LeokaEstetica.Platform.Models.Entities.Project;
using LeokaEstetica.Platform.Models.Entities.ProjectManagment;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.ProjectManagment;

public partial class ProjectTaskConfiguration : IEntityTypeConfiguration<ProjectTaskEntity>
{
    public void Configure(EntityTypeBuilder<ProjectTaskEntity> entity)
    {
        entity.ToTable("ProjectTasks", "ProjectManagment");

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
        
        entity.Property(e => e.Name)
            .HasColumnName("Name")
            .HasColumnType("varchar(255)")
            .IsRequired();
        
        entity.Property(e => e.Details)
            .HasColumnName("Details")
            .HasColumnType("text");
        
        entity.Property(e => e.WatcherIds)
            .HasColumnName("WatcherIds")
            .HasColumnType("bigint[]");
        
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

        entity.HasIndex(u => u.TaskId)
            .HasDatabaseName("PK_UserTasks_TaskId")
            .IsUnique();

        entity.Property(e => e.ProjectId)
            .HasColumnName("ProjectId")
            .HasColumnType("bigint")
            .IsRequired();
        
        entity.Property(e => e.ProjectTaskId)
            .HasColumnName("ProjectTaskId")
            .HasColumnType("bigint")
            .IsRequired();
        
        entity.Property(e => e.ResolutionId)
            .HasColumnName("ResolutionId")
            .HasColumnType("bigint");
        
        entity.Property(e => e.TagIds)
            .HasColumnName("TagIds")
            .HasColumnType("int[]");
        
        entity.Property(e => e.TaskTypeId)
            .HasColumnName("TaskTypeId")
            .HasColumnType("bigint")
            .IsRequired();
        
        entity.Property(e => e.ExecutorId)
            .HasColumnName("ExecutorId")
            .HasColumnType("bigint")
            .IsRequired();
        
        entity.HasOne(p => p.TaskStatus)
            .WithMany(b => b.ProjectTasks)
            .HasForeignKey(p => p.TaskStatusId)
            .HasConstraintName("FK_TaskStatuses_StatusId")
            .IsRequired();
        
        entity.HasOne(p => p.UserProject)
            .WithOne(b => b.ProjectTask)
            .HasForeignKey<UserProjectEntity>()
            .IsRequired();

        // entity.HasOne(p => p.TaskType)
        //     .WithOne(b => b.UserTask)
        //     .HasForeignKey<TaskTypeEntity>()
        //     .IsRequired();
        
        // entity.HasOne(p => p.TaskResolution)
        //     .WithOne(b => b.UserTask)
        //     .HasForeignKey<TaskResolutionEntity>()
        //     .IsRequired();
        
        entity.Property(e => e.PriorityId)
            .HasColumnName("PriorityId")
            .HasColumnType("int");

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<ProjectTaskEntity> entity);
}