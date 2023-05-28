using LeokaEstetica.Platform.Models.Entities.Profile;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.Profile;

public partial class ProfileInfoConfiguration : IEntityTypeConfiguration<ProfileInfoEntity>
{
    public void Configure(EntityTypeBuilder<ProfileInfoEntity> entity)
    {
        entity.ToTable("ProfilesInfo", "Profile");

        entity.HasKey(e => e.ProfileInfoId);

        entity.Property(e => e.ProfileInfoId)
            .HasColumnName("ProfileInfoId")
            .HasColumnType("bigserial");

        entity.Property(e => e.LastName)
            .HasColumnName("LastName")
            .HasColumnType("varchar(200)")
            .IsRequired();

        entity.Property(e => e.FirstName)
            .HasColumnName("FirstName")
            .HasColumnType("varchar(200)")
            .IsRequired();
        
        entity.Property(e => e.Patronymic)
            .HasColumnName("Patronymic")
            .HasColumnType("varchar(200)");
        
        entity.Property(e => e.IsShortFirstName)
            .HasColumnName("IsShortFirstName")
            .HasColumnType("bool")
            .IsRequired();
        
        entity.Property(e => e.Telegram)
            .HasColumnName("Telegram")
            .HasColumnType("varchar(200)");
        
        entity.Property(e => e.WhatsApp)
            .HasColumnName("WhatsApp")
            .HasColumnType("varchar(200)");
        
        entity.Property(e => e.Vkontakte)
            .HasColumnName("Vkontakte")
            .HasColumnType("varchar(200)");
        
        entity.Property(e => e.OtherLink)
            .HasColumnName("OtherLink")
            .HasColumnType("varchar(200)");
        
        entity.Property(e => e.Aboutme)
            .HasColumnName("Aboutme")
            .HasColumnType("text")
            .IsRequired();
        
        entity.Property(e => e.UserId)
            .HasColumnName("UserId")
            .HasColumnType("bigint")
            .IsRequired();
        
        entity.Property(e => e.Job)
            .HasColumnName("Job")
            .HasColumnType("varchar(200)")
            .IsRequired();

        entity.HasIndex(u => u.ProfileInfoId)
            .HasDatabaseName("PK_ProfilesInfo_ProfileInfoId")
            .IsUnique();

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<ProfileInfoEntity> entity);
}