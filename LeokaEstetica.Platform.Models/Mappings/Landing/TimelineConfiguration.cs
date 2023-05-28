using LeokaEstetica.Platform.Models.Entities.Landing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.Landing;

public partial class TimelineConfiguration : IEntityTypeConfiguration<TimelineEntity>
{
    public void Configure(EntityTypeBuilder<TimelineEntity> entity)
    {
        entity.ToTable("Timelines", "dbo");

        entity.HasKey(e => e.TimelineId);

        entity.Property(e => e.TimelineId)
            .HasColumnName("TimelineId")
            .HasColumnType("serial");

        entity.Property(e => e.TimelineText)
            .HasColumnName("TimelineText")
            .HasColumnType("text")
            .IsRequired();

        entity.Property(e => e.TimelineTitle)
            .HasColumnName("TimelineTitle")
            .HasColumnType("varchar(200)")
            .IsRequired();
        
        entity.Property(e => e.TimelineSysType)
            .HasColumnName("TimelineSysType")
            .HasColumnType("varchar(150)")
            .IsRequired();
        
        entity.Property(e => e.TimelineTypeName)
            .HasColumnName("TimelineTypeName")
            .HasColumnType("varchar(150)");
        
        entity.Property(e => e.Position)
            .HasColumnName("Position")
            .HasColumnType("smallint");

        entity.HasIndex(u => u.TimelineId)
            .HasDatabaseName("PK_TimelineId")
            .IsUnique();

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<TimelineEntity> entity);
}