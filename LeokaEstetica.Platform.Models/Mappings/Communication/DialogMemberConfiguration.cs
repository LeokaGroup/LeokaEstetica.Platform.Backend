using LeokaEstetica.Platform.Models.Entities.Communication;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.Communication;

public partial class DialogMemberConfiguration : IEntityTypeConfiguration<DialogMemberEntity>
{
    public void Configure(EntityTypeBuilder<DialogMemberEntity> entity)
    {
        entity.ToTable("DialogMembers", "Communications");

        entity.HasKey(e => e.MemberId);

        entity.Property(e => e.MemberId)
            .HasColumnName("MemberId")
            .HasColumnType("bigserial");

        entity.Property(e => e.Joined)
            .HasColumnName("Joined")
            .HasColumnType("timestamp")
            .IsRequired();
        
        entity.HasOne(p => p.Dialog)
            .WithMany(b => b.DialogMembers)
            .HasForeignKey(p => p.DialogId)
            .HasConstraintName("FK_MainInfoDialogs_DialogId");
        
        entity.HasOne(p => p.User)
            .WithMany(b => b.DialogMembers)
            .HasForeignKey(p => p.UserId)
            .HasConstraintName("FK_Users_UserId");

        entity.HasIndex(u => u.DialogId)
            .HasDatabaseName("PK_MainInfoDialogs_DialogId")
            .IsUnique();

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<DialogMemberEntity> entity);
}