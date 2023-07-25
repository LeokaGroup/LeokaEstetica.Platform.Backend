using LeokaEstetica.Platform.Models.Entities.Knowlege;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.Knowledge;

public partial class KnowledgeSubCategoryConfiguration : IEntityTypeConfiguration<KnowledgeSubCategoryEntity>
{
    public void Configure(EntityTypeBuilder<KnowledgeSubCategoryEntity> entity)
    {
        entity.ToTable("KnowledgeSubCategories", "Knowledge");

        entity.HasKey(e => e.SubCategoryId);

        entity.Property(e => e.SubCategoryId)
            .HasColumnName("SubCategoryId")
            .HasColumnType("bigserial");

        entity.Property(e => e.SubCategoryTypeSysName)
            .HasColumnName("SubCategoryTypeSysName")
            .HasColumnType("varchar(200)")
            .HasMaxLength(200)
            .IsRequired();

        entity.Property(e => e.Position)
            .HasColumnName("Position")
            .HasColumnType("int")
            .IsRequired();

        entity.Property(e => e.SubCategoryTypeName)
            .HasColumnName("SubCategoryTypeName")
            .HasColumnType("varchar(200)")
            .HasMaxLength(200)
            .IsRequired();

        entity.HasIndex(u => u.SubCategoryId)
            .HasDatabaseName("PK_KnowledgeSubCategories_SubCategoryId")
            .IsUnique();

        entity.HasOne(p => p.KnowledgeCategory)
            .WithMany(b => b.KnowledgeSubCategories)
            .HasForeignKey(p => p.CategoryId)
            .HasConstraintName("FK_KnowledgeCategories_CategoryId");
        
        entity.HasOne(p => p.KnowledgeSubCategoryTheme)
            .WithMany(b => b.KnowledgeSubCategories)
            .HasForeignKey(p => p.SubCategoryThemeId)
            .HasConstraintName("FK_KnowledgeSubCategoriesThemes_SubCategoryThemeId");

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<KnowledgeSubCategoryEntity> entity);
}