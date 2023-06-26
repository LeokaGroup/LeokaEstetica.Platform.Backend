using LeokaEstetica.Platform.Models.Entities.Ticket;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.Ticket;

public partial class TicketCategoryConfiguration : IEntityTypeConfiguration<TicketCategoryEntity>
{
    public void Configure(EntityTypeBuilder<TicketCategoryEntity> entity)
    {
        entity.ToTable("TicketCategories", "Communications");

        entity.HasKey(e => e.CategoryId);

        entity.Property(e => e.CategoryId)
            .HasColumnName("CategoryId")
            .HasColumnType("serial");
        
        entity.Property(e => e.CategoryName)
            .HasColumnName("CategoryName")
            .HasColumnType("varchar(150)")
            .HasMaxLength(150)
            .IsRequired();
        
        entity.Property(e => e.CategorySysName)
            .HasColumnName("CategorySysName")
            .HasColumnType("varchar(150)")
            .HasMaxLength(150)
            .IsRequired();
        
        entity.Property(e => e.Position)
            .HasColumnName("Position")
            .HasColumnType("smallint")
            .HasDefaultValue(0)
            .IsRequired();

        entity.HasIndex(u => u.CategoryId)
            .HasDatabaseName("PK_TicketCategories_CategoryId")
            .IsUnique();

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<TicketCategoryEntity> entity);
}