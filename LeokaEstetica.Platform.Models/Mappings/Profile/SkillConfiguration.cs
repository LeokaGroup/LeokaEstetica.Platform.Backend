using LeokaEstetica.Platform.Models.Entities.Profile;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.Profile;

public partial class SkillConfiguration : IEntityTypeConfiguration<SkillEntity>
{
     public void Configure(EntityTypeBuilder<SkillEntity> entity)
    {
        entity.ToTable("Skills", "Profile");

        entity.HasKey(e => e.SkillId);

        entity.Property(e => e.SkillId)
            .HasColumnName("SkillId")
            .HasColumnType("serial");

        entity.Property(e => e.SkillName)
            .HasColumnName("SkillName")
            .HasColumnType("varchar(200)")
            .IsRequired();

        entity.Property(e => e.SkillSysName)
            .HasColumnName("SkillSysName")
            .HasColumnType("varchar(200)")
            .IsRequired();
        
        entity.Property(e => e.Position)
            .HasColumnName("Position")
            .HasColumnType("int");

        entity.HasIndex(u => u.SkillId)
            .HasDatabaseName("PK_Skills_SkillId")
            .IsUnique();

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<SkillEntity> entity);
}