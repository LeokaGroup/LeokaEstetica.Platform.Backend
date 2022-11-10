using LeokaEstetica.Platform.Models.Entities.Vacancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.Vacancy;

public partial class VacancyConfiguration : IEntityTypeConfiguration<CatalogVacancyEntity>
{
    public void Configure(EntityTypeBuilder<CatalogVacancyEntity> entity)
    {
        entity.ToTable("CatalogVacancies", "Vacancies");

        entity.HasKey(e => e.VacancyId);

        entity.Property(e => e.VacancyId)
            .HasColumnName("VacancyId")
            .HasColumnType("bigserial");

        entity.Property(e => e.VacancyName)
            .HasColumnName("VacancyName")
            .HasColumnType("varchar(250)")
            .IsRequired();

        entity.Property(e => e.DateCreated)
            .HasColumnName("DateCreated")
            .HasColumnType("timestamp")
            .IsRequired();

        entity.Property(e => e.VacancyText)
            .HasColumnName("VacancyText")
            .HasColumnType("text")
            .IsRequired();
        
        entity.Property(e => e.WorkExperience)
            .HasColumnName("WorkExperience")
            .HasColumnType("varchar(100)");
        
        entity.Property(e => e.Payment)
            .HasColumnName("Payment")
            .HasColumnType("varchar(150)");
        
        entity.Property(e => e.Employment)
            .HasColumnName("Employment")
            .HasColumnType("varchar(200)");
        
        entity.Property(e => e.UserId)
            .HasColumnName("UserId")
            .HasColumnType("bigint")
            .IsRequired();

        entity.HasIndex(u => u.VacancyId)
            .HasDatabaseName("PK_Vacancies_VacancyId")
            .IsUnique();

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<CatalogVacancyEntity> entity);
}