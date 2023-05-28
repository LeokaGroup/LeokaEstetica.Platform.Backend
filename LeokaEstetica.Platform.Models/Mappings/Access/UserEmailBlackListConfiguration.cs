using LeokaEstetica.Platform.Models.Entities.Access;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.Access;

public partial class UserEmailBlackListConfiguration : IEntityTypeConfiguration<UserEmailBlackListEntity>
{
    public void Configure(EntityTypeBuilder<UserEmailBlackListEntity> entity)
    {
        entity.ToTable("UserEmailBlackList", "Access");

        entity.HasKey(e => e.BlackId);

        entity.Property(e => e.BlackId)
            .HasColumnName("BlackId")
            .HasColumnType("bigserial");

        entity.Property(e => e.UserId)
            .HasColumnName("UserId")
            .HasColumnType("bigint");
        
        entity.Property(e => e.Email)
            .HasColumnName("Email")
            .HasColumnType("varchar(120)")
            .HasMaxLength(120);

        entity.HasIndex(u => u.BlackId)
            .HasDatabaseName("PK_UserEmailBlackList_BlackId")
            .IsUnique();

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<UserEmailBlackListEntity> entity);
}