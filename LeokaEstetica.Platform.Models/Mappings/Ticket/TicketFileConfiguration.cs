using LeokaEstetica.Platform.Models.Entities.Ticket;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.Ticket;

public partial class TicketFileConfiguration : IEntityTypeConfiguration<TicketFileEntity>
{
    public void Configure(EntityTypeBuilder<TicketFileEntity> entity)
    {
        entity.ToTable("TicketFiles", "Communications");

        entity.HasKey(e => e.FileId);

        entity.Property(e => e.FileId)
            .HasColumnName("FileId")
            .HasColumnType("bigserial");
        
        entity.Property(e => e.Url)
            .HasColumnName("Url")
            .HasColumnType("text")
            .IsRequired();
        
        entity.Property(e => e.DateCreated)
            .HasColumnName("DateCreated")
            .HasColumnType("timestamp")
            .HasDefaultValue(DateTime.UtcNow)
            .IsRequired();
        
        entity.Property(e => e.Title)
            .HasColumnName("Title")
            .HasColumnType("varchar(150)");

        entity.Property(e => e.Description)
            .HasColumnName("Description")
            .HasColumnType("text");
        
        entity.Property(e => e.Position)
            .HasColumnName("Position")
            .HasColumnType("smallint")
            .HasDefaultValue(0)
            .IsRequired();
        
        entity.Property(e => e.Type)
            .HasColumnName("Type")
            .HasColumnType("TicketFileTypeEnum")
            .IsRequired();

        entity.HasIndex(u => u.FileId)
            .HasDatabaseName("PK_TicketFiles_FileId")
            .IsUnique();

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<TicketFileEntity> entity);
}