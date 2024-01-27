using LeokaEstetica.Platform.Models.Entities.ProjectManagment;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.ProjectManagment;

public partial class UserTaskTagConfiguration : IEntityTypeConfiguration<ProjectTagEntity>
{
    public void Configure(EntityTypeBuilder<ProjectTagEntity> entity)
    {
        entity.ToTable("UserTaskTags", "ProjectManagment");

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
        
        entity.Property(e => e.TagSysName)
            .HasColumnName("TagSysName")
            .HasColumnType("varchar(255)");
        
        entity.Property(e => e.TagDescription)
            .HasColumnName("TagDescription")
            .HasColumnType("varchar(255)");

        entity.HasIndex(u => u.TagId)
            .HasDatabaseName("PK_TaskTags_TagId")
            .IsUnique();

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<ProjectTagEntity> entity);
}