using LeokaEstetica.Platform.Models.Entities.Ticket;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.Ticket;

public partial class TicketMessageConfiguration : IEntityTypeConfiguration<TicketMessageEntity>
{
    public void Configure(EntityTypeBuilder<TicketMessageEntity> entity)
    {
        entity.ToTable("TicketMessages", "Communications");

        entity.HasKey(e => e.MessageId);

        entity.Property(e => e.MessageId)
            .HasColumnName("MessageId")
            .HasColumnType("bigserial");
        
        entity.Property(e => e.TicketId)
            .HasColumnName("TicketId")
            .HasColumnType("bigint")
            .IsRequired();
        
        entity.Property(e => e.UserId)
            .HasColumnName("UserId")
            .HasColumnType("bigint")
            .IsRequired();

        entity.Property(e => e.DateCreated)
            .HasColumnName("DateCreated")
            .HasColumnType("timestamp")
            .HasDefaultValue(DateTime.Now);
        
        entity.Property(e => e.Message)
            .HasColumnName("Message")
            .HasColumnType("text")
            .IsRequired();
        
        entity.Property(e => e.IsMyMessage)
            .HasColumnName("IsMyMessage")
            .HasColumnType("boolean")
            .IsRequired();

        entity.HasIndex(u => u.MessageId)
            .HasDatabaseName("PK_TicketMessages_MessageId")
            .IsUnique();
        
        entity.HasOne(p => p.MainInfoTicket)
            .WithMany(b => b.TicketMessages)
            .HasForeignKey(p => p.TicketId)
            .HasConstraintName("FK_TicketMessages_TicketId");
        
        entity.HasOne(p => p.User)
            .WithMany(b => b.TicketMessages)
            .HasForeignKey(p => p.UserId)
            .HasConstraintName("FK_Users_UserId");

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<TicketMessageEntity> entity);
}