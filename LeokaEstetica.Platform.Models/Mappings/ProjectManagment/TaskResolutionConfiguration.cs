using LeokaEstetica.Platform.Models.Entities.ProjectManagment;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.ProjectManagment;

public partial class TaskResolutionConfiguration : IEntityTypeConfiguration<TaskResolutionEntity>
{
    public void Configure(EntityTypeBuilder<TaskResolutionEntity> entity)
    {
        entity.ToTable("TaskResolutions", "ProjectManagment");

        entity.HasKey(e => e.ResolutionId);
        
        entity.Property(e => e.ResolutionId)
            .HasColumnName("ResolutionId")
            .HasColumnType("int")
            .IsRequired();
        
        entity.Property(e => e.ResolutionName)
            .HasColumnName("ResolutionName")
            .HasColumnType("varchar(150)")
            .IsRequired();
        
        entity.Property(e => e.ResolutionSysName)
            .HasColumnName("ResolutionSysName")
            .HasColumnType("varchar(150)")
            .IsRequired();
        
        entity.Property(e => e.Position)
            .HasColumnName("Position")
            .HasColumnType("int")
            .HasDefaultValue(0)
            .IsRequired();
        
        entity.HasIndex(u => u.ResolutionId)
            .HasDatabaseName("PK_TaskResolutions_ResolutionId")
            .IsUnique();

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<TaskResolutionEntity> entity);
}