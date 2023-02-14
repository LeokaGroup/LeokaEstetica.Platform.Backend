using LeokaEstetica.Platform.Models.Entities.Access;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.Access;

public partial class UserEmailBlackListShadowConfiguration : IEntityTypeConfiguration<UserEmailBlackListShadowEntity>
{
    public void Configure(EntityTypeBuilder<UserEmailBlackListShadowEntity> entity)
    {
        entity.ToTable("UserEmailBlackListShadow", "Access");

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
            .HasDefaultValue(DateTime.Now);

        entity.HasIndex(u => u.ShadowId)
            .HasDatabaseName("PK_UserEmailBlackListShadow_ShadowId")
            .IsUnique();

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<UserEmailBlackListShadowEntity> entity);
}