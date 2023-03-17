using LeokaEstetica.Platform.Models.Entities.Knowlege;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.Knowledge;

public partial class KnowledgeSubCategoryThemeConfogiration : IEntityTypeConfiguration<KnowledgeSubCategoryThemeEntity>
{
    public void Configure(EntityTypeBuilder<KnowledgeSubCategoryThemeEntity> entity)
    {
        entity.ToTable("KnowledgeSubCategoriesThemes", "Knowledge");

        entity.HasKey(e => e.SubCategoryThemeId);

        entity.Property(e => e.SubCategoryThemeId)
            .HasColumnName("SubCategoryThemeId")
            .HasColumnType("bigserial");

        entity.Property(e => e.SubCategoryThemeTitle)
            .HasColumnName("SubCategoryThemeTitle")
            .HasColumnType("varchar(200)")
            .HasMaxLength(200)
            .IsRequired();

        entity.Property(e => e.Position)
            .HasColumnName("Position")
            .HasColumnType("int")
            .IsRequired();

        entity.Property(e => e.SubCategoryThemeSubTitle)
            .HasColumnName("SubCategoryThemeSubTitle")
            .HasColumnType("varchar(200)")
            .HasMaxLength(200)
            .IsRequired();
        
        entity.Property(e => e.SubCategoryThemeText)
            .HasColumnName("SubCategoryThemeText")
            .HasColumnType("text")
            .IsRequired();
        
        entity.Property(e => e.SubCategoryThemeImg)
            .HasColumnName("SubCategoryThemeImg")
            .HasColumnType("text");

        entity.HasIndex(u => u.SubCategoryThemeId)
            .HasDatabaseName("PK_KnowledgeSubCategories_SubCategoryThemeId")
            .IsUnique();

        entity.HasOne(p => p.KnowledgeSubCategory)
            .WithMany(b => b.KnowledgeSubCategoryThemes)
            .HasForeignKey(p => p.SubCategoryId)
            .HasConstraintName("PK_KnowledgeSubCategories_SubCategoryThemeId");

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<KnowledgeSubCategoryThemeEntity> entity);
}