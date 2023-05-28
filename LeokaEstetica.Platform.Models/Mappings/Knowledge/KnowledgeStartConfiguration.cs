using LeokaEstetica.Platform.Models.Entities.Knowlege;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.Knowledge;

public partial class KnowledgeStartConfiguration : IEntityTypeConfiguration<KnowledgeStartEntity>
{
    public void Configure(EntityTypeBuilder<KnowledgeStartEntity> entity)
    {
        entity.ToTable("KnowledgeStart", "Knowledge");

        entity.HasKey(e => e.StartId);

        entity.Property(e => e.StartId)
            .HasColumnName("StartId")
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
        
        entity.Property(e => e.SubCategoryTitle)
            .HasColumnName("SubCategoryTitle")
            .HasColumnType("varchar(200)")
            .HasMaxLength(200)
            .IsRequired();
        
        entity.Property(e => e.Position)
            .HasColumnName("Position")
            .HasColumnType("int")
            .IsRequired();
        
        entity.Property(e => e.TopCategories)
            .HasColumnName("TopCategories")
            .HasColumnType("jsonb")
            .IsRequired();

        entity.HasIndex(u => u.StartId)
            .HasDatabaseName("PK_KnowledgeStart_StartId")
            .IsUnique();

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<KnowledgeStartEntity> entity);
}