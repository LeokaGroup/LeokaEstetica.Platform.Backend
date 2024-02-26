using LeokaEstetica.Platform.Models.Entities.ProjectTeam;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.ProjectTeam;

public partial class ProjectTeamConfiguration : IEntityTypeConfiguration<ProjectTeamEntity>
{
    public void Configure(EntityTypeBuilder<ProjectTeamEntity> entity)
    {
        entity.ToTable("ProjectsTeams", "Teams");

        entity.HasKey(e => e.TeamId);

        entity.Property(e => e.TeamId)
            .HasColumnName("TeamId")
            .HasColumnType("bigserial");
        
        entity.Property(e => e.ProjectId)
            .HasColumnName("ProjectId")
            .HasColumnType("bigint");
        
        entity.Property(e => e.Created)
            .HasColumnName("Created")
            .HasColumnType("timestamp with time zone");

        entity.HasOne(p => p.CatalogProject)
            .WithMany(b => b.ProjectsTeams)
            .HasForeignKey(p => p.ProjectId)
            .HasConstraintName("FK_Projects_ProjectId");

        entity.HasIndex(u => u.TeamId)
            .HasDatabaseName("PK_ProjectsTeams_TeamId")
            .IsUnique();

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<ProjectTeamEntity> entity);
}