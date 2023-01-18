using LeokaEstetica.Platform.Models.Entities.FareRule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.FareRule;

public partial class FareRuleItemConfiguration : IEntityTypeConfiguration<FareRuleItemEntity>
{
    public void Configure(EntityTypeBuilder<FareRuleItemEntity> entity)
    {
        entity.ToTable("FareRulesItems", "Rules");

        entity.HasKey(e => e.RuleItemId);

        entity.Property(e => e.RuleItemId)
            .HasColumnName("RuleItemId")
            .HasColumnType("serial");
        
        entity.Property(e => e.Name)
            .HasColumnName("Name")
            .HasColumnType("varchar(150)");
        
        entity.Property(e => e.Label)
            .HasColumnName("Label")
            .HasColumnType("varchar(150)");
        
        entity.Property(e => e.IsLater)
            .HasColumnName("IsLater")
            .HasColumnType("bool");
        
        entity.Property(e => e.Position)
            .HasColumnName("Position")
            .HasColumnType("int");

        entity.HasOne(p => p.FareRule)
            .WithMany(b => b.FareRuleItems)
            .HasForeignKey(p => p.RuleId)
            .HasConstraintName("FK_FareRules_RuleId");

        entity.HasIndex(u => u.RuleItemId)
            .HasDatabaseName("PK_FareRulesItems_RuleItemId")
            .IsUnique();

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<FareRuleItemEntity> entity);
}