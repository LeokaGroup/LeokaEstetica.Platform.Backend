using LeokaEstetica.Platform.Models.Entities.ProjectManagment;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.ProjectManagment;

public partial class TaskRelationConfiguration : IEntityTypeConfiguration<TaskRelationEntity>
{
    public void Configure(EntityTypeBuilder<TaskRelationEntity> entity)
    {
        entity.ToTable("TaskRelations", "ProjectManagment");

        entity.HasKey(e => new { e.RelationId, e.TaskId });
        
        entity.Property(e => e.RelationType)
            .HasColumnName("RelationType")
            .HasColumnType("varchar(150)")
            .IsRequired();
        
        entity.HasOne(p => p.UserTask)
            .WithMany(b => b.TaskRelations)
            .HasForeignKey(p => p.RelationId)
            .HasConstraintName("FK_UserTasks_TaskId")
            .IsRequired();

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<TaskRelationEntity> entity);
}