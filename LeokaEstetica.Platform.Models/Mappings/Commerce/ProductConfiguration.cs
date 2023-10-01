using LeokaEstetica.Platform.Models.Entities.Commerce;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.Commerce;

public partial class ProductConfiguration : IEntityTypeConfiguration<ProductEntity>
{
    public void Configure(EntityTypeBuilder<ProductEntity> entity)
    {
        entity.ToTable("Products", "Commerce");

        entity.HasKey(e => e.ProductId);

        entity.Property(e => e.ProductId)
            .HasColumnName("ProductId")
            .HasColumnType("bigserial");
        
        entity.Property(e => e.ProductName)
            .HasColumnName("ProductName")
            .HasColumnType("varchar(150)")
            .IsRequired();
        
        entity.Property(e => e.IsDiscount)
            .HasColumnName("IsDiscount")
            .HasColumnType("bool")
            .IsRequired();
        
        entity.Property(e => e.PercentDiscount)
            .HasColumnName("PercentDiscount")
            .HasColumnType("int")
            .IsRequired();
        
        entity.Property(e => e.RuleId)
            .HasColumnName("RuleId")
            .HasColumnType("int")
            .IsRequired();
        
        entity.Property(e => e.ProductType)
            .HasColumnName("ProductType")
            .HasColumnType("varchar(100)")
            .HasMaxLength(50);

        entity.Property(e => e.ProductPrice)
            .HasColumnName("ProductPrice")
            .HasColumnType("decimal(12,2)");

        entity.HasIndex(u => u.ProductId)
            .HasDatabaseName("PK_Products_ProductId")
            .IsUnique();

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<ProductEntity> entity);
}