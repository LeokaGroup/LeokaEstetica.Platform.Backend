using LeokaEstetica.Platform.Models.Entities.Landing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.Landing;

public partial class FonConfiguration : IEntityTypeConfiguration<FonEntity>
{
    public void Configure(EntityTypeBuilder<FonEntity> entity)
    {
        entity.ToTable("Fon", "dbo");

        entity.HasKey(e => e.FonId);

        entity.Property(e => e.FonId)
            .HasColumnName("FonId")
            .HasColumnType("serial");

        entity.Property(e => e.Text)
            .HasColumnName("Text")
            .HasColumnType("text")
            .IsRequired();

        entity.Property(e => e.Title)
            .HasColumnName("Title")
            .HasColumnType("varchar(200)")
            .IsRequired();

        entity.HasIndex(u => u.FonId)
            .HasDatabaseName("Fon_pkey")
            .IsUnique();

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<FonEntity> entity);
}