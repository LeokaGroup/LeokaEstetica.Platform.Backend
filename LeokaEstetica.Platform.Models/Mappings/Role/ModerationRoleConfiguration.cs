using LeokaEstetica.Platform.Models.Entities.Role;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.Role;

public partial class ModerationRoleConfiguration : IEntityTypeConfiguration<ModerationRoleEntity>
{
    public void Configure(EntityTypeBuilder<ModerationRoleEntity> entity)
    {
        entity.ToTable("ModerationRoles", "Roles");

        entity.HasKey(e => e.RoleId);

        entity.Property(e => e.RoleId)
            .HasColumnName("RoleId")
            .HasColumnType("serial");
        
        entity.Property(e => e.RoleName)
            .HasColumnName("RoleName")
            .HasColumnType("varchar(150)");
        
        entity.Property(e => e.RoleSysName)
            .HasColumnName("RoleSysName")
            .HasColumnType("varchar(150)");

        entity.HasIndex(u => u.RoleId)
            .HasDatabaseName("PK_ModerationRoles_RoleId")
            .IsUnique();

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<ModerationRoleEntity> entity);
}