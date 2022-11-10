using LeokaEstetica.Platform.Models.Entities.Vacancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.Vacancy;

public partial class CatalogVacancyConfiguration : IEntityTypeConfiguration<CatalogVacancyEntity>
{
    public void Configure(EntityTypeBuilder<CatalogVacancyEntity> entity)
    {
        entity.ToTable("CatalogVacancies", "Vacancies");

        entity.HasKey(e => e.VacancyId);

        entity.Property(e => e.VacancyId)
            .HasColumnName("VacancyId")
            .HasColumnType("bigserial");

        entity.HasOne(p => p.VacancyId)
            .WithMany(b => b.CatalogVacancies)
            .HasForeignKey(p => p.VacancyId)
            .HasConstraintName("FK_UserProjects_ProjectId");

        entity.HasIndex(u => u.VacancyId)
            .HasDatabaseName("PK_CatalogVacancies_VacancyId")
            .IsUnique();

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<CatalogVacancyEntity> entity);
}