using LeokaEstetica.Platform.Models.Entities.Vacancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.Vacancy;

public partial class CatalogVacancyConfiguration : IEntityTypeConfiguration<CatalogVacancyEntity>
{
    public void Configure(EntityTypeBuilder<CatalogVacancyEntity> entity)
    {
        entity.ToTable("CatalogVacancies", "Vacancies");

        entity.HasKey(e => e.CatalogVacancyId);

        entity.Property(e => e.CatalogVacancyId)
            .HasColumnName("CatalogVacancyId")
            .HasColumnType("bigserial");

        entity.HasOne(p => p.Vacancy)
            .WithMany(b => b.CatalogVacancies)
            .HasForeignKey(p => p.CatalogVacancyId)
            .HasConstraintName("FK_CatalogVacancies_VacancyId");

        entity.HasIndex(u => u.CatalogVacancyId)
            .HasDatabaseName("PK_CatalogVacancies_CatalogVacancyId")
            .IsUnique();

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<CatalogVacancyEntity> entity);
}