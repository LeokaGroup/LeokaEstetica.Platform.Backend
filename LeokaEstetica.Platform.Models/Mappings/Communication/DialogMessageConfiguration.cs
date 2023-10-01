using LeokaEstetica.Platform.Models.Entities.Communication;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.Communication;

public partial class DialogMessageConfiguration : IEntityTypeConfiguration<DialogMessageEntity>
{
    public void Configure(EntityTypeBuilder<DialogMessageEntity> entity)
    {
        entity.ToTable("DialogMessages", "Communications");

        entity.HasKey(e => e.MessageId);

        entity.Property(e => e.MessageId)
            .HasColumnName("MessageId")
            .HasColumnType("bigserial");
        
        entity.Property(e => e.Message)
            .HasColumnName("Message")
            .HasColumnType("text")
            .IsRequired();
        
        entity.Property(e => e.Created)
            .HasColumnName("Created")
            .HasColumnType("timestamp")
            .IsRequired();
        
        entity.Property(e => e.IsMyMessage)
            .HasColumnName("IsMyMessage")
            .HasColumnType("bool")
            .IsRequired();
        
        entity.HasOne(p => p.Dialog)
            .WithMany(b => b.DialogMessages)
            .HasForeignKey(p => p.DialogId)
            .HasConstraintName("FK_MainInfoDialogs_DialogId");
        
        entity.HasOne(p => p.User)
            .WithMany(b => b.DialogMessages)
            .HasForeignKey(p => p.UserId)
            .HasConstraintName("FK_Users_UserId");

        entity.HasIndex(u => u.MessageId)
            .HasDatabaseName("PK_DialogMessages_MessageId")
            .IsUnique();

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<DialogMessageEntity> entity);
}