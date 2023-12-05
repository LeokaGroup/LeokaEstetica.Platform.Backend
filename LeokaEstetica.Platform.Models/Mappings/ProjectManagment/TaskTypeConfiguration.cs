using LeokaEstetica.Platform.Models.Entities.ProjectManagment;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.ProjectManagment;

public partial class TaskTypeConfiguration : IEntityTypeConfiguration<TaskTypeEntity>
{
    public void Configure(EntityTypeBuilder<TaskTypeEntity> entity)
    {
        entity.ToTable("TaskTypes", "ProjectManagment");

        entity.HasKey(e => e.TypeId);
        
        entity.Property(e => e.TypeName)
            .HasColumnName("TypeName")
            .HasColumnType("varchar(150)")
            .IsRequired();
        
        entity.Property(e => e.TypeSysName)
            .HasColumnName("TypeSysName")
            .HasColumnType("varchar(150)")
            .IsRequired();
        
        entity.Property(e => e.Position)
            .HasColumnName("Position")
            .HasColumnType("int")
            .HasDefaultValue(0)
            .IsRequired();
        
        entity.HasIndex(u => u.TypeId)
            .HasDatabaseName("PK_TaskTypes_TypeId")
            .IsUnique();

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<TaskTypeEntity> entity);
}