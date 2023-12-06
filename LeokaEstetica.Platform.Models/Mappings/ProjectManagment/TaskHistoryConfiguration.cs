using LeokaEstetica.Platform.Models.Entities.ProjectManagment;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.ProjectManagment;

public partial class TaskHistoryConfiguration : IEntityTypeConfiguration<TaskHistoryEntity>
{
    public void Configure(EntityTypeBuilder<TaskHistoryEntity> entity)
    {
        entity.ToTable("TaskHistory", "ProjectManagment");

        entity.HasKey(e => e.HistoryId);

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

        entity.HasIndex(u => u.HistoryId)
            .HasDatabaseName("PK_TaskHistory_HistoryId")
            .IsUnique();

        entity.HasOne(p => p.ProjectTask)
            .WithMany(b => b.TaskHistories)
            .HasForeignKey(p => p.TaskId)
            .HasConstraintName("FK_UserTasks_TaskId")
            .IsRequired();
        
        entity.HasOne(p => p.HistoryAction)
            .WithMany(b => b.TaskHistories)
            .HasForeignKey(p => p.ActionId)
            .HasConstraintName("FK_HistoryActions_ActionId")
            .IsRequired();

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<TaskHistoryEntity> entity);
}