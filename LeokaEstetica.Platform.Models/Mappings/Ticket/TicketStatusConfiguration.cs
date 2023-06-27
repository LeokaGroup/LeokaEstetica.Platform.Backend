using LeokaEstetica.Platform.Models.Entities.Ticket;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.Ticket;

public partial class TicketStatusConfiguration : IEntityTypeConfiguration<TicketStatusEntity>
{
    public void Configure(EntityTypeBuilder<TicketStatusEntity> entity)
    {
        entity.ToTable("TicketStatuses", "Communications");

        entity.HasKey(e => e.StatusId);

        entity.Property(e => e.StatusId)
            .HasColumnName("StatusId")
            .HasColumnType("serial");
        
        entity.Property(e => e.StatusName)
            .HasColumnName("StatusName")
            .HasColumnType("varchar(50)")
            .HasMaxLength(50);
        
        entity.Property(e => e.StatusSysName)
            .HasColumnName("StatusSysName")
            .HasColumnType("varchar(50)")
            .HasMaxLength(50);

        entity.HasIndex(u => u.StatusId)
            .HasDatabaseName("PK_TicketStatuses_StatusId")
            .IsUnique();

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<TicketStatusEntity> entity);
}