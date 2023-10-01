using LeokaEstetica.Platform.Models.Entities.Access;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.Access;

public partial class UserVkBlackListShadowConfiguration : IEntityTypeConfiguration<UserVkBlackListShadowEntity>
{
    public void Configure(EntityTypeBuilder<UserVkBlackListShadowEntity> entity)
    {
        entity.ToTable("UserVkBlackListShadow", "Access");

        entity.HasKey(e => e.ShadowId);

        entity.Property(e => e.ShadowId)
            .HasColumnName("ShadowId")
            .HasColumnType("bigserial");

        entity.Property(e => e.UserId)
            .HasColumnName("UserId")
            .HasColumnType("bigint");
        
        entity.Property(e => e.ActionText)
            .HasColumnName("ActionText")
            .HasColumnType("text");
        
        entity.Property(e => e.ActionSysName)
            .HasColumnName("ActionSysName")
            .HasColumnType("varchar(100)");
        
        entity.Property(e => e.DateCreated)
            .HasColumnName("DateCreated")
            .HasColumnType("timestamp")
            .HasDefaultValue(DateTime.UtcNow);
        
        entity.Property(e => e.VkUserId)
            .HasColumnName("VkUserId")
            .HasColumnType("bigint");

        entity.HasIndex(u => u.ShadowId)
            .HasDatabaseName("PK_UserVkBlackListShadow_ShadowId")
            .IsUnique();

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<UserVkBlackListShadowEntity> entity);
}