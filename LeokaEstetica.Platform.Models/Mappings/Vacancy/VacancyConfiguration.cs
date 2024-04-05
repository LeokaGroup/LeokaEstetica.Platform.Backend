using LeokaEstetica.Platform.Models.Entities.Vacancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.Vacancy;

public partial class VacancyConfiguration : IEntityTypeConfiguration<UserVacancyEntity>
{
    public void Configure(EntityTypeBuilder<UserVacancyEntity> entity)
    {
        entity.ToTable("UserVacancies", "Vacancies");

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
            .HasColumnType("timestamp with time zone")
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
        
        entity.Property(e => e.Conditions)
            .HasColumnName("Conditions")
            .HasColumnType("text");
        
        entity.Property(e => e.Demands)
            .HasColumnName("Demands")
            .HasColumnType("text");

        entity.HasIndex(u => u.VacancyId)
            .HasDatabaseName("PK_Vacancies_VacancyId")
            .IsUnique();

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<UserVacancyEntity> entity);
}