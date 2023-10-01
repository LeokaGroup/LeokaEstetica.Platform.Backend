using LeokaEstetica.Platform.Models.Entities.Subscription;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.Subscription;

public partial class UserSubscriptionConfiguration : IEntityTypeConfiguration<UserSubscriptionEntity>
{
    public void Configure(EntityTypeBuilder<UserSubscriptionEntity> entity)
    {
        entity.ToTable("UserSubscriptions", "Subscriptions");

        entity.HasKey(e => e.UserSubscriptionId);

        entity.Property(e => e.UserSubscriptionId)
            .HasColumnName("UserSubscriptionId")
            .HasColumnType("serial");
        
        entity.Property(e => e.UserId)
            .HasColumnName("UserId")
            .HasColumnType("bigint");
        
        entity.Property(e => e.IsActive)
            .HasColumnName("IsActive")
            .HasColumnType("bool");
        
        entity.Property(e => e.MonthCount)
            .HasColumnName("MonthCount")
            .HasColumnType("tinyint");
        
        entity.Property(e => e.SubscriptionId)
            .HasColumnName("SubscriptionId")
            .HasColumnType("bigint");

        entity.HasIndex(u => u.UserSubscriptionId)
            .HasDatabaseName("PK_UserSubscriptions_UserSubscriptionId")
            .IsUnique();

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<UserSubscriptionEntity> entity);
}