using LeokaEstetica.Platform.Models.Entities.ProjectManagment;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.ProjectManagment;

public partial class TaskTagConfiguration : IEntityTypeConfiguration<TaskTagEntity>
{
    public void Configure(EntityTypeBuilder<TaskTagEntity> entity)
    {
        entity.ToTable("TaskTags", "ProjectManagment");

        entity.HasKey(e => e.TagId);

        entity.Property(e => e.TagId)
            .HasColumnName("TagId")
            .HasColumnType("serial");
        
        entity.Property(e => e.Position)
            .HasColumnName("Position")
            .HasColumnType("int")
            .HasDefaultValue(0)
            .IsRequired();
        
        entity.Property(e => e.TagName)
            .HasColumnName("TagName")
            .HasColumnType("varchar(150)")
            .IsRequired();
        
        entity.Property(e => e.TagSysName)
            .HasColumnName("TagSysName")
            .HasColumnType("varchar(150)")
            .IsRequired();

        entity.HasIndex(u => u.TagId)
            .HasDatabaseName("PK_TaskTags_TagId")
            .IsUnique();

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<TaskTagEntity> entity);
}