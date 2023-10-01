using LeokaEstetica.Platform.Models.Entities.Ticket;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.Ticket;

public partial class TicketMemberConfiguration : IEntityTypeConfiguration<TicketMemberEntity>
{
    public void Configure(EntityTypeBuilder<TicketMemberEntity> entity)
    {
        entity.ToTable("TicketMembers", "Communications");

        entity.HasKey(e => e.MemberId);

        entity.Property(e => e.MemberId)
            .HasColumnName("MemberId")
            .HasColumnType("bigserial");
        
        entity.Property(e => e.TicketId)
            .HasColumnName("TicketId")
            .HasColumnType("bigint")
            .IsRequired();
        
        entity.Property(e => e.UserId)
            .HasColumnName("UserId")
            .HasColumnType("bigint")
            .IsRequired();

        entity.Property(e => e.Joined)
            .HasColumnName("Joined")
            .HasColumnType("timestamp")
            .HasDefaultValue(DateTime.UtcNow);

        entity.HasIndex(u => u.MemberId)
            .HasDatabaseName("PK_TicketMembers_MemberId")
            .IsUnique();
        
        entity.HasOne(p => p.MainInfoTicket)
            .WithMany(b => b.TicketMembers)
            .HasForeignKey(p => p.TicketId)
            .HasConstraintName("FK_MainInfoTickets_TicketId");
        
        entity.HasOne(p => p.User)
            .WithMany(b => b.TicketMembers)
            .HasForeignKey(p => p.UserId)
            .HasConstraintName("FK_Users_UserId");

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<TicketMemberEntity> entity);
}