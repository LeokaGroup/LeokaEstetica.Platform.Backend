using LeokaEstetica.Platform.Models.Entities.Notification;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.Notification;

public partial class NotificationConfiguration : IEntityTypeConfiguration<NotificationEntity>
{
    public void Configure(EntityTypeBuilder<NotificationEntity> entity)
    {
        entity.ToTable("Notifications", "Notifications");

        entity.HasKey(e => e.NotificationId);

        entity.Property(e => e.NotificationId)
            .HasColumnName("NotificationId")
            .HasColumnType("bigserial");
        
        entity.Property(e => e.NotificationName)
            .HasColumnName("NotificationName")
            .HasColumnType("varchar(200)")
            .HasMaxLength(200)
            .IsRequired();
        
        entity.Property(e => e.NotificationSysName)
            .HasColumnName("NotificationSysName")
            .HasColumnType("varchar(200)")
            .HasMaxLength(200)
            .IsRequired();
        
        entity.Property(e => e.IsNeedAccepted)
            .HasColumnName("IsNeedAccepted")
            .HasColumnType("bool")
            .IsRequired();
        
        entity.Property(e => e.Approved)
            .HasColumnName("Approved")
            .HasColumnType("bool");
        
        entity.Property(e => e.Rejected)
            .HasColumnName("Rejected")
            .HasColumnType("bool");
        
        entity.Property(e => e.ProjectId)
            .HasColumnName("ProjectId")
            .HasColumnType("bigint");
        
        entity.Property(e => e.VacancyId)
            .HasColumnName("VacancyId")
            .HasColumnType("bigint");
        
        entity.Property(e => e.UserId)
            .HasColumnName("UserId")
            .HasColumnType("bigint")
            .IsRequired();
        
        entity.Property(e => e.NotificationText)
            .HasColumnName("NotificationText")
            .HasColumnType("text")
            .IsRequired();
        
        entity.Property(e => e.Created)
            .HasColumnName("Created")
            .HasColumnType("timestamp")
            .HasDefaultValue(DateTime.Now)
            .IsRequired();
        
        entity.Property(e => e.NotificationType)
            .HasColumnName("NotificationType")
            .HasColumnType("varchar(100)")
            .HasMaxLength(100);
        
        entity.Property(e => e.IsShow)
            .HasColumnName("IsShow")
            .HasColumnType("bool")
            .IsRequired();
        
        entity.Property(e => e.IsOwner)
            .HasColumnName("IsOwner")
            .HasColumnType("bool");

        entity.HasIndex(u => u.NotificationId)
            .HasDatabaseName("PK_Notifications_NotificationId")
            .IsUnique();

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<NotificationEntity> entity);
}