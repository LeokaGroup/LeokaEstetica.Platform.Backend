using LeokaEstetica.Platform.Models.Entities.Project;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.Project;

public partial class ProjectVacancyConfiguration : IEntityTypeConfiguration<ProjectVacancyEntity>
{
    public void Configure(EntityTypeBuilder<ProjectVacancyEntity> entity)
    {
        entity.ToTable("ProjectVacancies", "Projects");

        entity.HasKey(e => e.ProjectVacancyId);

        entity.Property(e => e.ProjectVacancyId)
            .HasColumnName("ProjectVacancyId")
            .HasColumnType("bigserial");
        
        entity.Property(e => e.ProjectId)
            .HasColumnName("ProjectId")
            .HasColumnType("bigint");

        entity.HasOne(p => p.UserVacancy)
            .WithMany(b => b.ProjectVacancies)
            .HasForeignKey(p => p.VacancyId)
            .HasConstraintName("FK_UserVacancies_VacancyId");

        entity.HasIndex(u => u.ProjectVacancyId)
            .HasDatabaseName("PK_ProjectVacancies_ProjectVacancyId")
            .IsUnique();

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<ProjectVacancyEntity> entity);
}