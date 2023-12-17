using LeokaEstetica.Platform.Models.Entities.Project;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.Project;

public partial class CatalogProjectConfiguration : IEntityTypeConfiguration<CatalogProjectEntity>
{
    public void Configure(EntityTypeBuilder<CatalogProjectEntity> entity)
    {
        entity.ToTable("CatalogProjects", "Projects");

        entity.HasKey(e => e.CatalogProjectId);

        entity.Property(e => e.CatalogProjectId)
            .HasColumnName("CatalogProjectId")
            .HasColumnType("bigserial");

        // entity.HasOne(p => p.Project)
        //     .WithMany(b => b.CatalogProjects)
        //     .HasForeignKey(p => p.ProjectId)
        //     .HasConstraintName("FK_UserProjects_ProjectId");

        entity.HasIndex(u => u.CatalogProjectId)
            .HasDatabaseName("PK_CatalogProjects_CatalogProjectId")
            .IsUnique();

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<CatalogProjectEntity> entity);
}