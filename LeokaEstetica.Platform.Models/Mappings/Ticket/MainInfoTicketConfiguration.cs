using LeokaEstetica.Platform.Models.Entities.Ticket;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.Ticket;

public partial class MainInfoTicketConfiguration : IEntityTypeConfiguration<MainInfoTicketEntity>
{
    public void Configure(EntityTypeBuilder<MainInfoTicketEntity> entity)
    {
        entity.ToTable("MainInfoTickets", "Communications");

        entity.HasKey(e => e.TicketId);

        entity.Property(e => e.TicketId)
            .HasColumnName("TicketId")
            .HasColumnType("bigserial");
        
        entity.Property(e => e.TicketName)
            .HasColumnName("TicketName")
            .HasColumnType("varchar(200)")
            .HasMaxLength(200);
        
        entity.Property(e => e.DateCreated)
            .HasColumnName("DateCreated")
            .HasColumnType("timestamp with time zone")
            .HasDefaultValue(DateTime.UtcNow);
        
        entity.Property(e => e.TicketStatusId)
            .HasColumnName("TicketStatusId")
            .HasColumnType("tinyint");
        
        entity.Property(e => e.TicketFileId)
            .HasColumnName("TicketFileId")
            .HasColumnType("bigint");

        entity.HasIndex(u => u.TicketId)
            .HasDatabaseName("PK_MainInfoTickets_TicketId")
            .IsUnique();

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<MainInfoTicketEntity> entity);
}