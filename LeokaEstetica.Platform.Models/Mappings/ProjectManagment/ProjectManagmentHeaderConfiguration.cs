using LeokaEstetica.Platform.Models.Entities.ProjectManagment;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.ProjectManagment;

public partial class ProjectManagmentHeaderConfiguration : IEntityTypeConfiguration<PanelEntity>
{
    public void Configure(EntityTypeBuilder<PanelEntity> entity)
    {
        entity.ToTable("panel_items", "ProjectManagment");

        entity.HasKey(e => e.PanelId);

        entity.Property(e => e.PanelId)
            .HasColumnName("PanelId")
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
        
        entity.Property(e => e.PanelType)
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
        
        entity.Property(e => e.ControlType)
            .HasColumnName("ControlType")
            .HasColumnType("varchar(100)")
            .IsRequired();
        
        entity.Property(e => e.Destination)
            .HasColumnName("Destination")
            .HasColumnType("varchar(100)")
            .IsRequired();

        entity.HasIndex(u => u.PanelId)
            .HasDatabaseName("PK_ProjectManagment_Header_HeaderId")
            .IsUnique();

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<PanelEntity> entity);
}