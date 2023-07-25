using LeokaEstetica.Platform.Models.Entities.Landing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.Landing;

public partial class PlatformOfferConfiguration : IEntityTypeConfiguration<PlatformOfferEntity>
{
    public void Configure(EntityTypeBuilder<PlatformOfferEntity> entity)
    {
        entity.ToTable("PlatformOffers", "dbo");

        entity.HasKey(e => e.OfferId);

        entity.Property(e => e.OfferId)
            .HasColumnName("OfferId")
            .HasColumnType("serial");

        entity.Property(e => e.OffeTitle)
            .HasColumnName("OffeTitle")
            .HasColumnType("varchar(200)")
            .IsRequired();

        entity.Property(e => e.OfferSubTitle)
            .HasColumnName("OfferSubTitle")
            .HasColumnType("varchar(200)")
            .IsRequired();

        entity.HasIndex(u => u.OfferId)
            .HasDatabaseName("PlatformOffers_pkey")
            .IsUnique();

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<PlatformOfferEntity> entity);
}