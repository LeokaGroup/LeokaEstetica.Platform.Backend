using LeokaEstetica.Platform.Models.Entities.Communication;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.Communication;

public partial class WisheOfferConfiguration : IEntityTypeConfiguration<WisheOfferEntity>
{
    public void Configure(EntityTypeBuilder<WisheOfferEntity> entity)
    {
        entity.ToTable("WishesOffers", "Communications");

        entity.HasKey(e => e.WisheOfferId);

        entity.Property(e => e.WisheOfferId)
            .HasColumnName("WisheOfferId")
            .HasColumnType("bigserial");
        
        entity.Property(e => e.ContactEmail)
            .HasColumnName("ContactEmail")
            .HasColumnType("varchar(200)")
            .IsRequired();
        
        entity.Property(e => e.WisheOfferText)
            .HasColumnName("WisheOfferText")
            .HasColumnType("text")
            .IsRequired();
        
        entity.Property(e => e.DateCreated)
            .HasColumnName("WisheOfferText")
            .HasColumnType("timestamp")
            .IsRequired();
        
        entity.HasIndex(u => u.WisheOfferId)
            .HasDatabaseName("PK_WishesOffers_WisheOfferId")
            .IsUnique();

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<WisheOfferEntity> entity);
}