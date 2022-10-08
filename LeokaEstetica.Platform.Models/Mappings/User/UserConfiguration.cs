using LeokaEstetica.Platform.Models.Entities.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.User;

public partial class UserConfiguration : IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> entity)
    {
        entity.ToTable("Users", "dbo");

        entity.HasKey(e => e.UserId);

        entity.Property(e => e.UserId)
            .HasColumnName("UserId")
            .HasColumnType("bigserial");

        entity.Property(e => e.LastName)
            .HasColumnName("LastName")
            .HasColumnType("varchar(120)");

        entity.Property(e => e.FirstName)
            .HasColumnName("FirstName")
            .HasColumnType("varchar(120)");

        entity.Property(e => e.SecondName)
            .HasColumnName("SecondName")
            .HasColumnType("varchar(120)");
        
        entity.Property(e => e.Login)
            .HasColumnName("Login")
            .HasColumnType("varchar(120)");
        
        entity.Property(e => e.UserIcon)
            .HasColumnName("UserIcon")
            .HasColumnType("text");
        
        entity.Property(e => e.DateRegister)
            .HasColumnName("DateRegister")
            .HasColumnType("timestamp")
            .IsRequired();
        
        entity.Property(e => e.Email)
            .HasColumnName("Email")
            .HasColumnType("varchar(120)")
            .IsRequired();
        
        entity.Property(e => e.EmailConfirmed)
            .HasColumnName("EmailConfirmed")
            .HasColumnType("bool")
            .IsRequired();
        
        entity.Property(e => e.PasswordHash)
            .HasColumnName("PasswordHash")
            .HasColumnType("text")
            .IsRequired();
        
        entity.Property(e => e.PhoneNumber)
            .HasColumnName("PhoneNumber")
            .HasColumnType("varchar(50)");
        
        entity.Property(e => e.PhoneNumberConfirmed)
            .HasColumnName("PhoneNumberConfirmed")
            .HasColumnType("bool")
            .IsRequired();
        
        entity.Property(e => e.TwoFactorEnabled)
            .HasColumnName("TwoFactorEnabled")
            .HasColumnType("bool")
            .IsRequired();
        
        entity.Property(e => e.LockoutEnd)
            .HasColumnName("LockoutEnd")
            .HasColumnType("bool")
            .IsRequired();
        
        entity.Property(e => e.LockoutEnabled)
            .HasColumnName("LockoutEnabled")
            .HasColumnType("bool")
            .IsRequired();
        
        entity.Property(e => e.UserCode)
            .HasColumnName("UserCode")
            .HasColumnType("uuid")
            .IsRequired();
        
        entity.Property(e => e.ConfirmEmailCode)
            .HasColumnName("ConfirmEmailCode")
            .HasColumnType("text");
        
        entity.Property(e => e.LockoutEnabledDate)
            .HasColumnName("LockoutEnabledDate")
            .HasColumnType("timestamp");
        
        entity.Property(e => e.LockoutEndDate)
            .HasColumnName("LockoutEndDate")
            .HasColumnType("timestamp");

        entity.HasIndex(u => u.UserId)
            .HasDatabaseName("PK_Users_UserId")
            .IsUnique();
        
        entity.HasIndex(u => u.UserCode)
            .HasDatabaseName("Uniq_Users_UserCode")
            .IsUnique();

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<UserEntity> entity);
}