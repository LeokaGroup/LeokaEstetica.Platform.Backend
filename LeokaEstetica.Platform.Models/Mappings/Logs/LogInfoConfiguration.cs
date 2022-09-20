using LeokaEstetica.Platform.Models.Entities.Log;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.Logs;

public partial class FonConfiguration : IEntityTypeConfiguration<LogInfoEntity>
{
    public void Configure(EntityTypeBuilder<LogInfoEntity> entity)
    {
        entity.ToTable("LogInfo", "Logs");

        entity.HasKey(e => e.LogId);

        entity.Property(e => e.LogId)
            .HasColumnName("LogId")
            .HasColumnType("bigserial");

        entity.Property(e => e.ExceptionMessage)
            .HasColumnName("ExceptionMessage")
            .HasColumnType("text")
            .IsRequired();

        entity.Property(e => e.DateCreated)
            .HasColumnName("DateCreated")
            .HasColumnType("timestamp with time zone")
            .IsRequired();
        
        entity.Property(e => e.Account)
            .HasColumnName("Account")
            .HasColumnType("varchar(100)")
            .IsRequired();
        
        entity.Property(e => e.StackTrace)
            .HasColumnName("StackTrace")
            .HasColumnType("text");
        
        entity.Property(e => e.LogKey)
            .HasColumnName("LogKey")
            .HasColumnType("uuid")
            .IsRequired();
        
        entity.Property(e => e.LogLevel)
            .HasColumnName("LogLevel")
            .HasColumnType("text")
            .IsRequired();

        entity.HasIndex(u => u.LogId)
            .HasDatabaseName("LogInfo_pkey")
            .IsUnique();
        
        entity.HasIndex(u => u.LogKey)
            .HasDatabaseName("FK_LogInfo_LogKey")
            .IsUnique();

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<LogInfoEntity> entity);
}