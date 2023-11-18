using LeokaEstetica.Platform.Models.Entities.ProjectManagment;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.ProjectManagment;

public partial class ViewStrategyConfiguration : IEntityTypeConfiguration<ViewStrategyEntity>
{
    public void Configure(EntityTypeBuilder<ViewStrategyEntity> entity)
    {
        entity.ToTable("ViewStrategies", "ProjectManagment");

        entity.HasKey(e => e.StrategyId);

        entity.Property(e => e.StrategyId)
            .HasColumnName("StrategyId")
            .HasColumnType("serial");
        
        entity.Property(e => e.ViewStrategyName)
            .HasColumnName("ViewStrategyName")
            .HasColumnType("varchar(255)");
        
        entity.Property(e => e.ViewStrategySysName)
            .HasColumnName("ViewStrategySysName")
            .HasColumnType("varchar(100)");

        entity.Property(e => e.Position)
            .HasColumnName("Position")
            .HasColumnType("int");

        entity.HasIndex(u => u.StrategyId)
            .HasDatabaseName("ViewStrategies_StrategyId")
            .IsUnique();

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<ViewStrategyEntity> entity);
}