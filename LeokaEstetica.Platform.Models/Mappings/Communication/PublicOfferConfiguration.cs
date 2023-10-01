using LeokaEstetica.Platform.Models.Entities.Communication;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.Communication;

public partial class PublicOfferConfiguration : IEntityTypeConfiguration<PublicOfferEntity>
{
    public void Configure(EntityTypeBuilder<PublicOfferEntity> entity)
    {
        entity.ToTable("PublicOffer", "Communications");

        entity.HasKey(e => e.OfferId);

        entity.Property(e => e.OfferId)
            .HasColumnName("OfferId")
            .HasColumnType("smallint");

        entity.Property(e => e.Name)
            .HasColumnName("Name")
            .HasColumnType("varchar(200)")
            .IsRequired();
        
        entity.Property(e => e.Description)
            .HasColumnName("Description")
            .HasColumnType("text")
            .IsRequired();

        entity.HasIndex(u => u.OfferId)
            .HasDatabaseName("PK_PublicOffer_OfferId")
            .IsUnique();

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<PublicOfferEntity> entity);
}