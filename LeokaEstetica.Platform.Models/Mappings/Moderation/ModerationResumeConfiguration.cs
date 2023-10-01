using LeokaEstetica.Platform.Models.Entities.Moderation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.Moderation;

public partial class ModerationResumeConfiguration : IEntityTypeConfiguration<ModerationResumeEntity>
{
    public void Configure(EntityTypeBuilder<ModerationResumeEntity> entity)
    {
        entity.ToTable("Resumes", "Moderation");

        entity.HasKey(e => e.ModerationId);

        entity.Property(e => e.ModerationId)
            .HasColumnName("ModerationId")
            .HasColumnType("bigserial");

        entity.Property(e => e.DateModeration)
            .HasColumnName("DateModeration")
            .HasColumnType("timestamp");

        entity.HasOne(p => p.ModerationStatus)
            .WithMany(b => b.ModerationResumes)
            .HasForeignKey(p => p.ModerationStatusId)
            .HasConstraintName("FK_ModerationStatuses_StatusId");
        
        entity.HasOne(p => p.ProfileInfo)
            .WithMany(b => b.ModerationResumes)
            .HasForeignKey(p => p.ProfileInfoId)
            .HasConstraintName("FK_ProfilesInfo_ProfileInfoId");

        entity.HasIndex(u => u.ModerationId)
            .HasDatabaseName("PK_Resumes_ModerationId")
            .IsUnique();

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<ModerationResumeEntity> entity);
}