using LeokaEstetica.Platform.Models.Entities.ProjectManagment;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.ProjectManagment;

public partial class HistoryActionConfiguration : IEntityTypeConfiguration<HistoryActionEntity>
{
    public void Configure(EntityTypeBuilder<HistoryActionEntity> entity)
    {
        entity.ToTable("HistoryActions", "ProjectManagment");

        entity.HasKey(e => e.ActionId);
        
        entity.Property(e => e.ActionId)
            .HasColumnName("ActionId")
            .HasColumnType("int")
            .IsRequired();
        
        entity.Property(e => e.ActionName)
            .HasColumnName("ActionName")
            .HasColumnType("varchar(150)")
            .IsRequired();
        
        entity.Property(e => e.ActionSysName)
            .HasColumnName("ActionSysName")
            .HasColumnType("varchar(150)")
            .IsRequired();
        
        entity.Property(e => e.Position)
            .HasColumnName("Position")
            .HasColumnType("int")
            .HasDefaultValue(0)
            .IsRequired();

        entity.HasIndex(u => u.ActionId)
            .HasDatabaseName("PK_HistoryActions_ActionId")
            .IsUnique();

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<HistoryActionEntity> entity);
}