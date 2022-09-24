using LeokaEstetica.Platform.Models.Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.Common;

public partial class HeaderConfiguration : IEntityTypeConfiguration<HeaderEntity>
{
    public void Configure(EntityTypeBuilder<HeaderEntity> entity)
    {
        entity.ToTable("Header", "dbo");

        entity.HasKey(e => e.HeaderId);

        entity.Property(e => e.HeaderId)
            .HasColumnName("HeaderId")
            .HasColumnType("serial");

        entity.Property(e => e.MenuItemTitle)
            .HasColumnName("HeaderMenuItemTitle")
            .HasColumnType("varchar(200)")
            .IsRequired();

        entity.Property(e => e.MenuItemUrl)
            .HasColumnName("HeaderMenuItemUrl")
            .HasColumnType("varchar(200)");
        
        entity.Property(e => e.Position)
            .HasColumnName("Position")
            .HasColumnType("int")
            .IsRequired()
            .HasDefaultValue(0);

        entity.HasIndex(u => u.HeaderId)
            .HasDatabaseName("PK_Header_HeaderId")
            .IsUnique();

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<HeaderEntity> entity);
}