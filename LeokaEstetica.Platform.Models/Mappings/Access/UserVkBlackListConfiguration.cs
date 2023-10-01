using LeokaEstetica.Platform.Models.Entities.Access;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.Access;

public partial class UserVkBlackListConfiguration : IEntityTypeConfiguration<UserVkBlackListEntity>
{
    public void Configure(EntityTypeBuilder<UserVkBlackListEntity> entity)
    {
        entity.ToTable("UserVkBlackList", "Access");

        entity.HasKey(e => e.BlackId);

        entity.Property(e => e.BlackId)
            .HasColumnName("BlackId")
            .HasColumnType("bigserial");

        entity.Property(e => e.UserId)
            .HasColumnName("UserId")
            .HasColumnType("bigint");
        
        entity.Property(e => e.VkUserId)
            .HasColumnName("VkUserId")
            .HasColumnType("bigint");

        entity.HasIndex(u => u.BlackId)
            .HasDatabaseName("PK_UserVkBlackList_BlackId")
            .IsUnique();

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<UserVkBlackListEntity> entity);
}