using LeokaEstetica.Platform.Models.Entities.FareRule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.FareRule;

public partial class DiscountRuleConfiguration : IEntityTypeConfiguration<DiscountRuleEntity>
{
    public void Configure(EntityTypeBuilder<DiscountRuleEntity> entity)
    {
        entity.ToTable("DiscountRules", "Rules");

        entity.HasCheckConstraint("DiscountRules_Check_Month", "Month > 0 AND Month <= 12");

        entity.HasKey(e => e.RuleId);

        entity.Property(e => e.RuleId)
            .HasColumnName("RuleId")
            .HasColumnType("serial");
        
        entity.Property(e => e.Month)
            .HasColumnName("Month")
            .HasColumnType("smallint")
            .IsRequired();

        entity.Property(e => e.Percent)
            .HasColumnName("Percent")
            .HasColumnType("decimal(5,4)")
            .IsRequired();
        
        entity.Property(e => e.Price)
            .HasColumnName("Price")
            .HasColumnType("decimal(12,2)");
        
        entity.Property(e => e.Type)
            .HasColumnName("Type")
            .HasColumnType("varchar(150)")
            .IsRequired();
        
        entity.Property(e => e.RussianName)
            .HasColumnName("RussianName")
            .HasColumnType("varchar(150)")
            .IsRequired();

        entity.HasIndex(u => u.RuleId)
            .HasDatabaseName("PK_DiscountRules_RuleId")
            .IsUnique();

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<DiscountRuleEntity> entity);
}