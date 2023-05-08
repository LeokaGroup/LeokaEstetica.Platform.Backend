using LeokaEstetica.Platform.Models.Entities.FareRule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.FareRule;

public partial class FareRuleConfiguration : IEntityTypeConfiguration<FareRuleEntity>
{
    public void Configure(EntityTypeBuilder<FareRuleEntity> entity)
    {
        entity.ToTable("FareRules", "Rules");

        entity.HasKey(e => e.RuleId);

        entity.Property(e => e.RuleId)
            .HasColumnName("RuleId")
            .HasColumnType("serial");
        
        entity.Property(e => e.Name)
            .HasColumnName("Name")
            .HasColumnType("varchar(150)");

        entity.Property(e => e.Label)
            .HasColumnName("Label")
            .HasColumnType("varchar(200)");
        
        entity.Property(e => e.Position)
            .HasColumnName("Position")
            .HasColumnType("int");
        
        entity.Property(e => e.Price)
            .HasColumnName("Price")
            .HasColumnType("decimal(12,2)");
        
        entity.Property(e => e.Currency)
            .HasColumnName("Currency")
            .HasColumnType("varchar(5)");
        
        entity.Property(e => e.IsPopular)
            .HasColumnName("IsPopular")
            .HasColumnType("bool");
        
        entity.Property(e => e.IsFree)
            .HasColumnName("IsFree")
            .HasColumnType("bool");
        
        entity.Property(e => e.PublicId)
            .HasColumnName("PublicId")
            .HasColumnType("uuid")
            .HasDefaultValue(Guid.NewGuid())
            .IsRequired();

        entity.HasIndex(u => u.RuleId)
            .HasDatabaseName("PK_FareRules_RuleId")
            .IsUnique();

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<FareRuleEntity> entity);
}