using LeokaEstetica.Platform.Models.Entities.Moderation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.Moderation;

public partial class ModerationProjectConfiguration : IEntityTypeConfiguration<ModerationProjectEntity>
{
    public void Configure(EntityTypeBuilder<ModerationProjectEntity> entity)
    {
        entity.ToTable("Projects", "Moderation");

        entity.HasKey(e => e.ModerationId);

        entity.Property(e => e.ModerationId)
            .HasColumnName("ModerationId")
            .HasColumnType("bigserial");
        
        entity.Property(e => e.ProjectId)
            .HasColumnName("ProjectId")
            .HasColumnType("bigint");
        
        entity.Property(e => e.DateModeration)
            .HasColumnName("DateModeration")
            .HasColumnType("timestamp");
        
        entity.HasOne(p => p.UserProject)
            .WithMany(b => b.ModerationProjects)
            .HasForeignKey(p => p.ProjectId)
            .HasConstraintName("FK_Projects_ProjectId");

        entity.HasIndex(u => u.ModerationId)
            .HasDatabaseName("PK_Projects_ModerationId")
            .IsUnique();

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<ModerationProjectEntity> entity);
}