using LeokaEstetica.Platform.Models.Entities.ProjectManagment;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.ProjectManagment;

public partial class TaskPriorityConfiguration : IEntityTypeConfiguration<TaskPriorityEntity>
{
    public void Configure(EntityTypeBuilder<TaskPriorityEntity> entity)
    {
        entity.ToTable("TaskPriorities", "ProjectManagment");

        entity.HasKey(e => e.PriorityId);

        entity.Property(e => e.PriorityName)
            .HasColumnName("PriorityName")
            .HasColumnType("varchar(50)")
            .IsRequired();
        
        entity.Property(e => e.PrioritySysName)
            .HasColumnName("PrioritySysName")
            .HasColumnType("varchar(50)")
            .IsRequired();
        
        entity.Property(e => e.Position)
            .HasColumnName("Position")
            .HasColumnType("int")
            .HasDefaultValue(0)
            .IsRequired();

        entity.HasIndex(u => u.PriorityId)
            .HasDatabaseName("PK_TaskPriorities_PriorityId")
            .IsUnique();

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<TaskPriorityEntity> entity);
}