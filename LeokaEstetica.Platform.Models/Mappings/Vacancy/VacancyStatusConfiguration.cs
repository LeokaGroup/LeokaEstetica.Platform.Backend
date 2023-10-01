using LeokaEstetica.Platform.Models.Entities.Vacancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.Vacancy;

public partial class VacancyStatusConfiguration : IEntityTypeConfiguration<VacancyStatusEntity>
{
    public void Configure(EntityTypeBuilder<VacancyStatusEntity> entity)
    {
        entity.ToTable("VacancyStatuses", "Vacancies");

        entity.HasKey(e => e.StatusId);

        entity.Property(e => e.StatusId)
            .HasColumnName("StatusId")
            .HasColumnType("serial");
        
        entity.Property(e => e.VacancyStatusSysName)
            .HasColumnName("VacancyStatusSysName")
            .HasColumnType("varchar(100)");
        
        entity.Property(e => e.VacancyStatusName)
            .HasColumnName("VacancyStatusName")
            .HasColumnType("varchar(100)");

        entity.HasOne(p => p.UserVacancy)
            .WithMany(b => b.VacancyStatuses)
            .HasForeignKey(p => p.VacancyId)
            .HasConstraintName("FK_UserVacancies_VacancyId")
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasIndex(u => u.StatusId)
            .HasDatabaseName("PK_VacancyStatuses_StatusId")
            .IsUnique();

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<VacancyStatusEntity> entity);
}