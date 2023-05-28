using LeokaEstetica.Platform.Models.Entities.Profile;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.Profile;

public partial class UserSkillConfiguration : IEntityTypeConfiguration<UserSkillEntity>
{
    public void Configure(EntityTypeBuilder<UserSkillEntity> entity)
    {
        entity.ToTable("UserSkills", "Profile");

        entity.HasKey(e => e.UserSkillId);

        entity.Property(e => e.UserSkillId)
            .HasColumnName("UserSkillId")
            .HasColumnType("bigserial");

        entity.Property(e => e.SkillId)
            .HasColumnName("SkillId")
            .HasColumnType("int")
            .IsRequired();

        entity.Property(e => e.UserId)
            .HasColumnName("UserId")
            .HasColumnType("int")
            .IsRequired();
        
        entity.Property(e => e.Position)
            .HasColumnName("Position")
            .HasColumnType("int");

        entity.HasIndex(u => u.UserSkillId)
            .HasDatabaseName("PK_UserSkills_UserSkillId")
            .IsUnique();

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<UserSkillEntity> entity);
}