using LeokaEstetica.Platform.Models.Entities.Moderation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.Moderation;

public partial class ResumeRemarkConfiguration : IEntityTypeConfiguration<ResumeRemarkEntity>
{
    public void Configure(EntityTypeBuilder<ResumeRemarkEntity> entity)
    {
        entity.ToTable("ProfileRemarks", "Moderation");

        entity.HasKey(e => e.RemarkId);

        entity.Property(e => e.RemarkId)
            .HasColumnName("RemarkId")
            .HasColumnType("bigserial")
            .IsRequired();

        entity.Property(e => e.ProfileInfoId)
            .HasColumnName("ProfileInfoId")
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
            .HasDefaultValue(string.Empty)
            .IsRequired();

        entity.HasIndex(u => u.RemarkId)
            .HasDatabaseName("PK_RemarkId")
            .IsUnique();
        
        entity.HasOne(p => p.ProfileInfo)
            .WithMany(b => b.ResumeRemarks)
            .HasForeignKey(p => p.ProfileInfoId)
            .HasConstraintName("FK_Profile_ProfileInfoId");
        
        entity.HasOne(p => p.ModerationUser)
            .WithMany(b => b.ResumeRemarks)
            .HasForeignKey(p => p.ModerationUserId)
            .HasConstraintName("FK_Users_UserId_ModerationUserId");
        
        entity.HasOne(p => p.RemarkStatuse)
            .WithMany(b => b.ResumeRemarks)
            .HasForeignKey(p => p.RemarkStatusId)
            .HasConstraintName("FK_Moderation_RemarksStatuses_RemarkStatusId");

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<ResumeRemarkEntity> entity);
}