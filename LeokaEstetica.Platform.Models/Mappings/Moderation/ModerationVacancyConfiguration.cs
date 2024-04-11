using LeokaEstetica.Platform.Models.Entities.Moderation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.Moderation;

public partial class ModerationVacancyConfiguration : IEntityTypeConfiguration<ModerationVacancyEntity>
{
    public void Configure(EntityTypeBuilder<ModerationVacancyEntity> entity)
    {
        entity.ToTable("Vacancies", "Moderation");

        entity.HasKey(e => e.ModerationId);

        entity.Property(e => e.ModerationId)
            .HasColumnName("ModerationId")
            .HasColumnType("bigserial");
        
        entity.Property(e => e.VacancyId)
            .HasColumnName("VacancyId")
            .HasColumnType("bigint");
        
        entity.Property(e => e.DateModeration)
            .HasColumnName("DateModeration")
            .HasColumnType("timestamp");
        
        entity.Property(e => e.ModerationStatusId)
            .HasColumnName("ModerationStatusId")
            .HasColumnType("int");
        
        entity.HasOne(p => p.UserVacancy)
            .WithMany(b => b.ModerationVacancy)
            .HasForeignKey(p => p.VacancyId)
            .HasConstraintName("FK_Vacancies_VacancyId");
        
        entity.HasOne(p => p.ModerationStatus)
            .WithMany(b => b.ModerationVacancies)
            .HasForeignKey(p => p.ModerationStatusId)
            .HasConstraintName("FK_ModerationStatuses_StatusId");

        entity.HasIndex(u => u.ModerationId)
            .HasDatabaseName("PK_Vacancies_ModerationId")
            .IsUnique();

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<ModerationVacancyEntity> entity);
}