using LeokaEstetica.Platform.Models.Entities.Knowlege;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.Knowledge;

public partial class KnowledgeCategoryConfiguration : IEntityTypeConfiguration<KnowledgeCategoryEntity>
{
    public void Configure(EntityTypeBuilder<KnowledgeCategoryEntity> entity)
    {
        entity.ToTable("KnowledgeCategories", "Knowledge");

        entity.HasKey(e => e.CategoryId);

        entity.Property(e => e.CategoryId)
            .HasColumnName("CategoryId")
            .HasColumnType("bigserial");

        entity.Property(e => e.CategoryTitle)
            .HasColumnName("CategoryTitle")
            .HasColumnType("varchar(200)")
            .HasMaxLength(200)
            .IsRequired();

        entity.Property(e => e.CategoryTypeSysName)
            .HasColumnName("CategoryTypeSysName")
            .HasColumnType("varchar(200)")
            .HasMaxLength(200)
            .IsRequired();

        entity.Property(e => e.SubCategoryId)
            .HasColumnName("SubCategoryId")
            .HasColumnType("bigint")
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

        entity.Property(e => e.IsTop)
            .HasColumnName("IsTop")
            .HasColumnType("bool")
            .IsRequired();

        entity.HasIndex(u => u.CategoryId)
            .HasDatabaseName("PK_KnowledgeCategories_CategoryId")
            .IsUnique();

        entity.HasOne(p => p.KnowledgeStart)
            .WithMany(b => b.KnowledgeCategories)
            .HasForeignKey(p => p.StartId)
            .HasConstraintName("FK_KnowledgeStart_StartId");

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<KnowledgeCategoryEntity> entity);
}