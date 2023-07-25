using LeokaEstetica.Platform.Models.Entities.Vacancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.Vacancy;

public partial class VacancyMenuItemConfiguration : IEntityTypeConfiguration<VacancyMenuItemEntity>
{
    public void Configure(EntityTypeBuilder<VacancyMenuItemEntity> entity)
    {
        entity.ToTable("VacancyMenuItems", "Vacancies");

        entity.HasKey(e => e.VacancyMenuItemId);

        entity.Property(e => e.VacancyMenuItemId)
            .HasColumnName("VacancyMenuItemId")
            .HasColumnType("serial");

        entity.Property(e => e.VacancyMenuItems)
            .HasColumnName("VacancyMenuItems")
            .HasColumnType("jsonb")
            .IsRequired();

        entity.HasIndex(u => u.VacancyMenuItemId)
            .HasDatabaseName("PK_VacancyMenuItems_VacancyMenuItemId")
            .IsUnique();

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<VacancyMenuItemEntity> entity);
}