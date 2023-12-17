using LeokaEstetica.Platform.Models.Entities.Project;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.Project;

public partial class ArchivedProjectConfiguration : IEntityTypeConfiguration<ArchivedProjectEntity>
{
    public void Configure(EntityTypeBuilder<ArchivedProjectEntity> entity)
    {
        entity.ToTable("ArchivedProjects", "Projects");

        entity.HasKey(e => e.ArchiveId);

        entity.Property(e => e.ArchiveId)
            .HasColumnName("ArchiveId")
            .HasColumnType("bigserial");

        entity.Property(e => e.ProjectId)
            .HasColumnName("ProjectId")
            .HasColumnType("bigint")
            .IsRequired();

        entity.Property(e => e.UserId)
            .HasColumnName("UserId")
            .HasColumnType("bigint")
            .IsRequired();

        entity.Property(e => e.DateArchived)
           .HasColumnName("DateArchived")
           .HasColumnType("timestamp")
           .HasDefaultValue(DateTime.UtcNow)
           .IsRequired();

        // entity.HasOne(a => a.UserProject)
        //    .WithMany(u => u.ArchivedProjects)
        //    .HasForeignKey(a => a.ProjectId)
        //    .HasConstraintName("FK_UserProjects_ProjectId")
        //    .OnDelete(DeleteBehavior.Cascade);

        entity.HasIndex(a => a.ArchiveId)
            .HasDatabaseName("PK_ArchivedProjects_ArchiveId")
            .IsUnique();

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<ArchivedProjectEntity> entity);
}
