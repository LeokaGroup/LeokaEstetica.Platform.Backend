using LeokaEstetica.Platform.Models.Entities.Ticket;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.Ticket;

public partial class UserTicketRoleConfiguration : IEntityTypeConfiguration<UserTicketRoleEntity>
{
    public void Configure(EntityTypeBuilder<UserTicketRoleEntity> entity)
    {
        entity.ToTable("UserTicketRoles", "Communications");

        entity.HasKey(e => e.UserRoleId);

        entity.Property(e => e.UserRoleId)
            .HasColumnName("UserRoleId")
            .HasColumnType("bigserial");
        
        entity.Property(e => e.RoleId)
            .HasColumnName("RoleId")
            .HasColumnType("bigint")
            .IsRequired();
        
        entity.Property(e => e.UserId)
            .HasColumnName("UserId")
            .HasColumnType("bigint")
            .IsRequired();

        entity.HasIndex(u => u.UserRoleId)
            .HasDatabaseName("PK_UserTicketRoles_UserRoleId")
            .IsUnique();
        
        entity.HasOne(p => p.TicketRole)
            .WithMany(b => b.UserTicketRoles)
            .HasForeignKey(p => p.RoleId)
            .HasConstraintName("FK_TicketRoles_RoleId");
        
        entity.HasOne(p => p.User)
            .WithMany(b => b.UserTicketRoles)
            .HasForeignKey(p => p.UserId)
            .HasConstraintName("FK_Users_UserId");

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<UserTicketRoleEntity> entity);
}