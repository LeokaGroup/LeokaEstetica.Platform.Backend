using LeokaEstetica.Platform.Models.Entities.Access;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.Access;

public partial class UserPhoneBlackListConfiguration : IEntityTypeConfiguration<UserPhoneBlackListEntity>
{
    public void Configure(EntityTypeBuilder<UserPhoneBlackListEntity> entity)
    {
        entity.ToTable("UserPhoneBlackList", "Access");

        entity.HasKey(e => e.BlackId);

        entity.Property(e => e.BlackId)
            .HasColumnName("BlackId")
            .HasColumnType("bigserial");

        entity.Property(e => e.UserId)
            .HasColumnName("UserId")
            .HasColumnType("bigint");
        
        entity.Property(e => e.PhoneNumber)
            .HasColumnName("PhoneNumber")
            .HasColumnType("varchar(50)")
            .HasMaxLength(50);

        entity.HasIndex(u => u.BlackId)
            .HasDatabaseName("PK_UserPhoneBlackList_BlackId")
            .IsUnique();

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<UserPhoneBlackListEntity> entity);
}