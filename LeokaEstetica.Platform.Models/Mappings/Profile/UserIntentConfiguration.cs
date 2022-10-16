using LeokaEstetica.Platform.Models.Entities.Profile;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.Profile;

public partial class UserIntentConfiguration : IEntityTypeConfiguration<UserIntentEntity>
{
    public void Configure(EntityTypeBuilder<UserIntentEntity> entity)
    {
        entity.ToTable("UserIntents", "Profile");

        entity.HasKey(e => e.UserIntentId);

        entity.Property(e => e.UserIntentId)
            .HasColumnName("UserIntentId")
            .HasColumnType("bigserial");

        entity.Property(e => e.IntentId)
            .HasColumnName("IntentId")
            .HasColumnType("int")
            .IsRequired();

        entity.Property(e => e.UserId)
            .HasColumnName("UserId")
            .HasColumnType("bigint")
            .IsRequired();
        
        entity.Property(e => e.Position)
            .HasColumnName("Position")
            .HasColumnType("int");

        entity.HasIndex(u => u.IntentId)
            .HasDatabaseName("PK_Intents_IntentId")
            .IsUnique();

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<UserIntentEntity> entity);
}