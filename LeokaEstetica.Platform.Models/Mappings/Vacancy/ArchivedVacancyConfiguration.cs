using LeokaEstetica.Platform.Models.Entities.Vacancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.Vacancy;

public partial class ArchivedVacancyConfiguration : IEntityTypeConfiguration<ArchivedVacancyEntity>
{
    public void Configure(EntityTypeBuilder<ArchivedVacancyEntity> entity)
    {
        entity.ToTable("ArchivedVacancies", "Vacancies");

        entity.HasKey(e => e.ArchiveId);

        entity.Property(e => e.ArchiveId)
            .HasColumnName("ArchiveId")
            .HasColumnType("bigserial");

        entity.Property(e => e.VacancyId)
            .HasColumnName("VacancyId")
            .HasColumnType("bigint")
            .IsRequired();

        entity.Property(e => e.DateArchived)
           .HasColumnName("DateArchived")
           .HasColumnType("timestamp with time zone")
           .HasDefaultValue(DateTime.UtcNow)
           .IsRequired();

        entity.HasOne(a => a.UserVacancy)
           .WithMany(u => u.ArchivedVacancies)
           .HasForeignKey(a => a.VacancyId)
           .HasConstraintName("FK_UserVacancies_VacancyId");

        entity.HasIndex(a => a.ArchiveId)
            .HasDatabaseName("PK_ArchivedVacancies_ArchiveId")
            .IsUnique();

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<ArchivedVacancyEntity> entity);
}

