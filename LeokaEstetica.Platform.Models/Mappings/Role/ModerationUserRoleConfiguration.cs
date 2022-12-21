using LeokaEstetica.Platform.Models.Entities.Role;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.Role;

public partial class ModerationUserRoleConfiguration : IEntityTypeConfiguration<ModerationUserRoleEntity>
{
    public void Configure(EntityTypeBuilder<ModerationUserRoleEntity> entity)
    {
        entity.ToTable("ModerationRoles", "Roles");

        entity.HasKey(e => e.UserRoleId);

        entity.Property(e => e.UserRoleId)
            .HasColumnName("UserRoleId")
            .HasColumnType("serial");
        
        entity.Property(e => e.RoleId)
            .HasColumnName("RoleId")
            .HasColumnType("int");
        
        entity.Property(e => e.UserId)
            .HasColumnName("UserId")
            .HasColumnType("bigint");
        
        entity.Property(e => e.IsActive)
            .HasColumnName("IsActive")
            .HasColumnType("bool");

        entity.HasIndex(u => u.UserRoleId)
            .HasDatabaseName("PK_ModerationUserRoles_UserRoleId")
            .IsUnique();

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<ModerationUserRoleEntity> entity);
}