using LeokaEstetica.Platform.Models.Entities.Profile;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.Profile;

public partial class IntentConfiguration : IEntityTypeConfiguration<IntentEntity>
{
    public void Configure(EntityTypeBuilder<IntentEntity> entity)
    {
        entity.ToTable("Intents", "Profile");

        entity.HasKey(e => e.IntentId);

        entity.Property(e => e.IntentId)
            .HasColumnName("IntentId")
            .HasColumnType("serial");

        entity.Property(e => e.IntentName)
            .HasColumnName("IntentName")
            .HasColumnType("varchar(200)")
            .IsRequired();

        entity.Property(e => e.IntentSysName)
            .HasColumnName("IntentSysName")
            .HasColumnType("varchar(200)")
            .IsRequired();
        
        entity.Property(e => e.Position)
            .HasColumnName("Position")
            .HasColumnType("int");

        entity.HasIndex(u => u.IntentId)
            .HasDatabaseName("PK_Intents_IntentId")
            .IsUnique();

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<IntentEntity> entity);
}