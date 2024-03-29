using LeokaEstetica.Platform.Models.Entities.Moderation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.Moderation;

public partial class ProjectRemarkConfiguration : IEntityTypeConfiguration<ProjectRemarkEntity>
{
    public void Configure(EntityTypeBuilder<ProjectRemarkEntity> entity)
    {
        entity.ToTable("ProjectsRemarks", "Moderation");

        entity.HasKey(e => e.RemarkId);

        entity.Property(e => e.RemarkId)
            .HasColumnName("RemarkId")
            .HasColumnType("bigserial")
            .IsRequired();

        entity.Property(e => e.ProjectId)
            .HasColumnName("ProjectId")
            .HasColumnType("bigint")
            .IsRequired();

        entity.Property(e => e.FieldName)
            .HasColumnName("FieldName")
            .HasColumnType("varchar(150)")
            .HasMaxLength(150)
            .IsRequired();
        
        entity.Property(e => e.RemarkText)
            .HasColumnName("RemarkText")
            .HasColumnType("varchar(500)")
            .HasMaxLength(500)
            .IsRequired();
        
        entity.Property(e => e.RussianName)
            .HasColumnName("RussianName")
            .HasColumnType("varchar(500)")
            .HasMaxLength(500)
            .IsRequired();
        
        entity.Property(e => e.ModerationUserId)
            .HasColumnName("ModerationUserId")
            .HasColumnType("int")
            .IsRequired();
        
        entity.Property(e => e.DateCreated)
            .HasColumnName("DateCreated")
            .HasColumnType("timestamp")
            .IsRequired();
        
        entity.Property(e => e.RemarkStatusId)
            .HasColumnName("RemarkStatusId")
            .HasColumnType("int")
            .IsRequired();
        
        entity.Property(e => e.RejectReason)
            .HasColumnName("RejectReason")
            .HasColumnType("varchar(300)")
            .HasMaxLength(300)
            .HasDefaultValue(string.Empty)
            .IsRequired();

        entity.HasIndex(u => u.RemarkId)
            .HasDatabaseName("PK_RemarkId")
            .IsUnique();
        
        entity.HasOne(p => p.UserProject)
            .WithMany(b => b.ProjectRemarks)
            .HasForeignKey(p => p.ProjectId)
            .HasConstraintName("FK_Projects_UserProjects_ProjectId")
            .OnDelete(DeleteBehavior.Cascade);
        
        entity.HasOne(p => p.ModerationUser)
            .WithMany(b => b.ProjectRemarks)
            .HasForeignKey(p => p.ModerationUserId)
            .HasConstraintName("FK_Users_UserId_ModerationUserId");
        
        entity.HasOne(p => p.RemarkStatuse)
            .WithMany(b => b.ProjectRemarks)
            .HasForeignKey(p => p.RemarkStatusId)
            .HasConstraintName("FK_Moderation_RemarksStatuses_RemarkStatusId");

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<ProjectRemarkEntity> entity);
}