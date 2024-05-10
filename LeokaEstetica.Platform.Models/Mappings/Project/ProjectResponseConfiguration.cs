using LeokaEstetica.Platform.Models.Entities.Project;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.Project;

public partial class ProjectResponseConfiguration : IEntityTypeConfiguration<ProjectResponseEntity>
{
    public void Configure(EntityTypeBuilder<ProjectResponseEntity> entity)
    {
        entity.ToTable("ProjectResponses", "Projects");

        entity.HasKey(e => e.ResponseId);

        entity.Property(e => e.ProjectId)
            .HasColumnName("ProjectId")
            .HasColumnType("bigserial")
            .IsRequired();

        entity.Property(e => e.UserId)
            .HasColumnName("UserId")
            .HasColumnType("bigserial")
            .IsRequired();

        entity.Property(e => e.DateResponse)
            .HasColumnName("DateResponse")
            .HasColumnType("timestamp with time zone")
            .HasDefaultValue(DateTime.UtcNow)
            .IsRequired();
        
        entity.Property(e => e.ProjectResponseStatuseId)
            .HasColumnName("ProjectResponseStatuseId")
            .HasColumnType("serial")
            .IsRequired();
        
        entity.Property(e => e.VacancyId)
            .HasColumnName("VacancyId")
            .HasColumnType("bigint");

        entity.HasIndex(u => u.ResponseId)
            .HasDatabaseName("PK_ProjectResponses_ResponseId")
            .IsUnique();

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<ProjectResponseEntity> entity);
}