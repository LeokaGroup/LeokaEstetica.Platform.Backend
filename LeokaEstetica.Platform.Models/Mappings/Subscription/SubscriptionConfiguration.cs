using LeokaEstetica.Platform.Models.Entities.Subscription;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.Subscription;

public partial class SubscriptionConfiguration : IEntityTypeConfiguration<SubscriptionEntity>
{
    public void Configure(EntityTypeBuilder<SubscriptionEntity> entity)
    {
        entity.ToTable("Subscriptions", "Subscriptions");

        entity.HasKey(e => e.SubscriptionId);

        entity.Property(e => e.SubscriptionId)
            .HasColumnName("SubscriptionId")
            .HasColumnType("bigserial");
        
        entity.Property(e => e.ObjectId)
            .HasColumnName("ObjectId")
            .HasColumnType("bigint");
        
        entity.Property(e => e.SubscriptionType)
            .HasColumnName("SubscriptionType")
            .HasColumnType("varchar(100)");
        
        entity.Property(e => e.IsLatter)
            .HasColumnName("IsLatter")
            .HasColumnType("bool");

        entity.HasIndex(u => u.SubscriptionId)
            .HasDatabaseName("PK_Subscriptions_SubscriptionId")
            .IsUnique();

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<SubscriptionEntity> entity);
}