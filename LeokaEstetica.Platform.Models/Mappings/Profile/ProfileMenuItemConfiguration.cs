using LeokaEstetica.Platform.Models.Entities.Profile;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.Profile;

public partial class ProfileMenuItemConfiguration : IEntityTypeConfiguration<ProfileMenuItemEntity>
{
    public void Configure(EntityTypeBuilder<ProfileMenuItemEntity> entity)
    {
        entity.ToTable("ProfileMenuItems", "Profile");

        entity.HasKey(e => e.ProfileMenuItemId);

        entity.Property(e => e.ProfileMenuItemId)
            .HasColumnName("ProfileMenuItemId")
            .HasColumnType("serial");

        entity.Property(e => e.ProfileMenuItems)
            .HasColumnName("ProfileMenuItems")
            .HasColumnType("jsonb")
            .IsRequired();

        entity.HasIndex(u => u.ProfileMenuItemId)
            .HasDatabaseName("PK_ProfileMenuItems_ProfileMenuItemId")
            .IsUnique();

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<ProfileMenuItemEntity> entity);
}