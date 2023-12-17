using LeokaEstetica.Platform.Models.Entities.ProjectManagment;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.ProjectManagment;

public partial class TaskDependencyConfiguration : IEntityTypeConfiguration<TaskDependencyEntity>
{
    public void Configure(EntityTypeBuilder<TaskDependencyEntity> entity)
    {
        entity.ToTable("TaskDependencies", "ProjectManagment");

        entity.HasKey(e => e.DependencyId);
        
        entity.Property(e => e.DependencySysType)
            .HasColumnName("DependencySysType")
            .HasColumnType("varchar(150)")
            .IsRequired();
        
        entity.Property(e => e.DependencyTypeName)
            .HasColumnName("DependencyTypeName")
            .HasColumnType("varchar(150)")
            .IsRequired();
        
        entity.Property(e => e.Position)
            .HasColumnName("Position")
            .HasColumnType("int")
            .HasDefaultValue(0)
            .IsRequired();
        
        entity.HasOne(p => p.ProjectTask)
            .WithMany(b => b.TaskDependencies)
            .HasForeignKey(p => p.TaskId)
            .HasConstraintName("FK_UserTasks_TaskId")
            .IsRequired();

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<TaskDependencyEntity> entity);
}