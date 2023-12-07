using LeokaEstetica.Platform.Models.Entities.ProjectManagment;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.ProjectManagment;

public partial class TaskStatusConfiguration : IEntityTypeConfiguration<TaskStatusEntity>
{
    public void Configure(EntityTypeBuilder<TaskStatusEntity> entity)
    {
        entity.ToTable("TaskStatuses", "ProjectManagment");

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
        
        entity.Property(e => e.Position)
            .HasColumnName("Position")
            .HasColumnType("int")
            .HasDefaultValue(0)
            .IsRequired();

        entity.HasIndex(u => u.StatusId)
            .HasDatabaseName("PK_TaskStatuses_StatusId")
            .IsUnique();

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<TaskStatusEntity> entity);
}