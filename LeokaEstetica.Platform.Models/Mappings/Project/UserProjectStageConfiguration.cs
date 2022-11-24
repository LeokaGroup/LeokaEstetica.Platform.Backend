using LeokaEstetica.Platform.Models.Entities.Project;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.Project;

public partial class UserProjectStageConfiguration : IEntityTypeConfiguration<UserProjectStageEntity>
{
    public void Configure(EntityTypeBuilder<UserProjectStageEntity> entity)
    {
        entity.ToTable("UserProjectsStages", "Projects");

        entity.HasKey(e => e.UserProjectStageId);

        entity.Property(e => e.UserProjectStageId)
            .HasColumnName("UserProjectStageId")
            .HasColumnType("bigserial");

        entity.Property(e => e.StageId)
            .HasColumnName("StageId")
            .HasColumnType("int")
            .IsRequired();

        entity.HasIndex(u => u.StageId)
            .HasDatabaseName("PK_ProjectStages_StageId")
            .IsUnique();
        
        entity.HasOne(p => p.UserProject)
            .WithMany(b => b.UserProjectsStages)
            .HasForeignKey(p => p.ProjectId)
            .HasConstraintName("FK_CatalogVacancies_VacancyId");

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<UserProjectStageEntity> entity);
}