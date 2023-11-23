using LeokaEstetica.Platform.Models.Entities.ProjectManagment;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.ProjectManagment;

public partial class ProjectManagmentHeaderConfiguration : IEntityTypeConfiguration<ProjectManagmentHeaderEntity>
{
    public void Configure(EntityTypeBuilder<ProjectManagmentHeaderEntity> entity)
    {
        entity.ToTable("Header", "ProjectManagment");

        entity.HasKey(e => e.HeaderId);

        entity.Property(e => e.HeaderId)
            .HasColumnName("HeaderId")
            .HasColumnType("serial")
            .IsRequired();
        
        entity.Property(e => e.ItemName)
            .HasColumnName("ItemName")
            .HasColumnType("varchar(200)")
            .IsRequired();
        
        entity.Property(e => e.ItemUrl)
            .HasColumnName("ItemUrl")
            .HasColumnType("text");

        entity.Property(e => e.Position)
            .HasColumnName("Position")
            .HasColumnType("int");
        
        entity.Property(e => e.HeaderType)
            .HasColumnName("HeaderType")
            .HasColumnType("varchar(50)")
            .IsRequired();
        
        entity.Property(e => e.Items)
            .HasColumnName("Items")
            .HasColumnType("jsonb")
            .IsRequired();
        
        entity.Property(e => e.HasItems)
            .HasColumnName("HasItems")
            .HasColumnType("bool")
            .IsRequired();
        
        entity.Property(e => e.IsDisabled)
            .HasColumnName("IsDisabled")
            .HasColumnType("bool")
            .IsRequired();

        entity.HasIndex(u => u.HeaderId)
            .HasDatabaseName("PK_ProjectManagment_Header_HeaderId")
            .IsUnique();

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<ProjectManagmentHeaderEntity> entity);
}