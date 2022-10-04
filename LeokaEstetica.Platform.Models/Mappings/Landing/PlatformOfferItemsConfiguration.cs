using LeokaEstetica.Platform.Models.Entities.Landing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.Landing;

public partial class PlatformOfferItemsConfiguration : IEntityTypeConfiguration<PlatformOfferItemsEntity>
{
    public void Configure(EntityTypeBuilder<PlatformOfferItemsEntity> entity)
    {
        entity.ToTable("PlatformOffersItems", "dbo");

        entity.HasKey(e => e.ItemId);

        entity.Property(e => e.ItemId)
            .HasColumnName("ItemId")
            .HasColumnType("serial");

        entity.Property(e => e.ItemText)
            .HasColumnName("ItemText")
            .HasColumnType("varchar(400)")
            .IsRequired();

        entity.Property(e => e.ItemIcon)
            .HasColumnName("ItemIcon")
            .HasColumnType("text");
        
        entity.Property(e => e.Position)
            .HasColumnName("Position")
            .HasColumnType("int");

        entity.HasIndex(u => u.ItemId)
            .HasDatabaseName("PlatformOffersItems_pkey")
            .IsUnique();

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<PlatformOfferItemsEntity> entity);
}