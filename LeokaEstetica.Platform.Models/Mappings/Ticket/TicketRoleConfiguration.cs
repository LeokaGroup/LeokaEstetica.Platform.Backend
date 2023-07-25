using LeokaEstetica.Platform.Models.Entities.Ticket;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.Ticket;

public partial class TicketRoleConfiguration : IEntityTypeConfiguration<TicketRoleEntity>
{
    public void Configure(EntityTypeBuilder<TicketRoleEntity> entity)
    {
        entity.ToTable("TicketRoles", "Communications");

        entity.HasKey(e => e.RoleId);

        entity.Property(e => e.RoleId)
            .HasColumnName("RoleId")
            .HasColumnType("serial");

        entity.Property(e => e.RoleName)
            .HasColumnName("RoleName")
            .HasColumnType("varchar(100)")
            .IsRequired();
        
        entity.Property(e => e.RoleSysName)
            .HasColumnName("RoleSysName")
            .HasColumnType("varchar(100)")
            .IsRequired();

        entity.HasIndex(u => u.RoleId)
            .HasDatabaseName("PK_TicketRoles_RoleId")
            .IsUnique();

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<TicketRoleEntity> entity);
}