using LeokaEstetica.Platform.Models.Entities.ProjectTeam;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.ProjectTeam;

public partial class ProjectTeamMemberConfiguration : IEntityTypeConfiguration<ProjectTeamMemberEntity>
{
    public void Configure(EntityTypeBuilder<ProjectTeamMemberEntity> entity)
    {
        entity.ToTable("ProjectsTeamsMembers", "Teams");

        entity.HasKey(e => e.MemberId);

        entity.Property(e => e.MemberId)
            .HasColumnName("MemberId")
            .HasColumnType("bigserial");
        
        entity.Property(e => e.VacancyId)
            .HasColumnName("VacancyId")
            .HasColumnType("bigint")
            .IsRequired();
        
        entity.Property(e => e.Joined)
            .HasColumnName("Joined")
            .HasColumnType("timestamp")
            .IsRequired();

        entity.HasOne(p => p.ProjectTeam)
            .WithMany(b => b.ProjectTeamMembers)
            .HasForeignKey(p => p.TeamId)
            .HasConstraintName("FK_ProjectsTeams_TeamId");
        
        entity.HasOne(p => p.User)
            .WithMany(b => b.ProjectTeamMembers)
            .HasForeignKey(p => p.UserId)
            .HasConstraintName("FK_Users_UserId");
        
        entity.HasOne(p => p.CatalogVacancy)
            .WithMany(b => b.ProjectTeamMembers)
            .HasForeignKey(p => p.VacancyId)
            .HasConstraintName("FK_Users_UserId");

        entity.HasIndex(u => u.MemberId)
            .HasDatabaseName("PK_ProjectsTeamsMembers_MemberId")
            .IsUnique();

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<ProjectTeamMemberEntity> entity);
}