using LeokaEstetica.Platform.Models.Entities.Project;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.Project;

public partial class ProjectStageConfiguration : IEntityTypeConfiguration<ProjectStageEntity>
{
    public void Configure(EntityTypeBuilder<ProjectStageEntity> entity)
    {
        entity.ToTable("ProjectStages", "Projects");

        entity.HasKey(e => e.StageId);

        entity.Property(e => e.StageId)
            .HasColumnName("StageId")
            .HasColumnType("serial");
        
        entity.Property(e => e.StageName)
            .HasColumnName("StageName")
            .HasColumnType("varchar(150)")
            .IsRequired();

        entity.Property(e => e.StageSysName)
            .HasColumnName("StageSysName")
            .HasColumnType("varchar(150)")
            .IsRequired();
        
        entity.Property(e => e.Position)
            .HasColumnName("Position")
            .HasColumnType("int")
            .IsRequired();

        entity.HasIndex(u => u.StageId)
            .HasDatabaseName("PK_ProjectStages_StageId")
            .IsUnique();

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<ProjectStageEntity> entity);
}