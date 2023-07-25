using LeokaEstetica.Platform.Models.Entities.Access;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.Access;

public partial class UserPhoneBlackListShadowConfiguration : IEntityTypeConfiguration<UserPhoneBlackListShadowEntity>
{
    public void Configure(EntityTypeBuilder<UserPhoneBlackListShadowEntity> entity)
    {
        entity.ToTable("UserPhoneBlackListShadow", "Access");

        entity.HasKey(e => e.ShadowId);

        entity.Property(e => e.ShadowId)
            .HasColumnName("ShadowId")
            .HasColumnType("bigserial");

        entity.Property(e => e.UserId)
            .HasColumnName("UserId")
            .HasColumnType("bigint");
        
        entity.Property(e => e.ActionText)
            .HasColumnName("ActionText")
            .HasColumnType("varchar(300)");
        
        entity.Property(e => e.PhoneNumber)
            .HasColumnName("PhoneNumber")
            .HasColumnType("varchar(50)");
        
        entity.Property(e => e.DateCreated)
            .HasColumnName("DateCreated")
            .HasColumnType("timestamp")
            .HasDefaultValue(DateTime.UtcNow);

        entity.HasIndex(u => u.ShadowId)
            .HasDatabaseName("PK_UserPhoneBlackListShadow_ShadowId")
            .IsUnique();

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<UserPhoneBlackListShadowEntity> entity);
}