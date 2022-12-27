using LeokaEstetica.Platform.Models.Entities.ProjectTeam;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.ProjectTeam;

public partial class ProjectTeamVacancyConfiguration : IEntityTypeConfiguration<ProjectTeamVacancyEntity>
{
    public void Configure(EntityTypeBuilder<ProjectTeamVacancyEntity> entity)
    {
        entity.ToTable("ProjectsTeamsVacancies", "Teams");

        entity.HasKey(e => e.TeamVacancyId);

        entity.Property(e => e.TeamVacancyId)
            .HasColumnName("TeamVacancyId")
            .HasColumnType("bigserial");
        
        entity.Property(e => e.VacancyId)
            .HasColumnName("ProjectId")
            .HasColumnType("bigint")
            .IsRequired();
        
        entity.Property(e => e.IsActive)
            .HasColumnName("IsActive")
            .HasColumnType("bool")
            .IsRequired();

        entity.HasOne(p => p.CatalogVacancy)
            .WithMany(b => b.ProjectTeamVacancies)
            .HasForeignKey(p => p.VacancyId)
            .HasConstraintName("FK_CatalogVacancies_VacancyId");
        
        entity.HasOne(p => p.ProjectTeam)
            .WithMany(b => b.ProjectTeamVacancies)
            .HasForeignKey(p => p.TeamId)
            .HasConstraintName("FK_ProjectsTeams_TeamId");

        entity.HasIndex(u => u.TeamVacancyId)
            .HasDatabaseName("PK_ProjectsTeamsVacancies_TeamVacancyId")
            .IsUnique();

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<ProjectTeamVacancyEntity> entity);
}