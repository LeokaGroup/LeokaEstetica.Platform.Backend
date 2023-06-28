using LeokaEstetica.Platform.Models.Entities.Moderation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.Moderation;

public partial class ModerationUserConfiguration : IEntityTypeConfiguration<ModerationUserEntity>
{
    public void Configure(EntityTypeBuilder<ModerationUserEntity> entity)
    {
        entity.ToTable("Users", "Moderation");

        entity.HasKey(e => e.Id);

        entity.Property(e => e.Id)
            .HasColumnName("Id")
            .HasColumnType("bigserial");
        
        entity.Property(e => e.UserId)
            .HasColumnName("UserId")
            .HasColumnType("bigint")
            .IsRequired();
        
        entity.Property(e => e.DateCreated)
            .HasColumnName("DateCreated")
            .HasColumnType("timestamp")
            .HasDefaultValue(DateTime.UtcNow)
            .IsRequired();
        
        entity.Property(e => e.UserRoleId)
            .HasColumnName("UserRoleId")
            .HasColumnType("int")
            .IsRequired();
        
        entity.Property(e => e.PasswordHash)
            .HasColumnName("PasswordHash")
            .HasColumnType("text")
            .IsRequired();

        entity.HasIndex(u => u.Id)
            .HasDatabaseName("PK_Users_Id")
            .IsUnique();
        
        entity.HasOne(p => p.User)
            .WithMany(b => b.ModerationUsers)
            .HasForeignKey(p => p.UserId)
            .HasConstraintName("FK_Users_UserId");
        
        entity.HasOne(p => p.ModerationRole)
            .WithMany(b => b.ModerationUsers)
            .HasForeignKey(p => p.UserRoleId)
            .HasConstraintName("FK_Roles_RoleId");

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<ModerationUserEntity> entity);
}