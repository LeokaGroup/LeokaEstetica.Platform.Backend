using LeokaEstetica.Platform.Models.Entities.Communication;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.Communication;

public partial class ContactConfiguration : IEntityTypeConfiguration<ContactEntity>
{
    public void Configure(EntityTypeBuilder<ContactEntity> entity)
    {
        entity.ToTable("Contacts", "Communications");

        entity.HasKey(e => e.ContactId);

        entity.Property(e => e.ContactId)
            .HasColumnName("MemberId")
            .HasColumnType("smallint");

        entity.Property(e => e.Name)
            .HasColumnName("Name")
            .HasColumnType("varchar(200)")
            .IsRequired();
        
        entity.Property(e => e.Description)
            .HasColumnName("Description")
            .HasColumnType("text")
            .IsRequired();

        entity.HasIndex(u => u.ContactId)
            .HasDatabaseName("PK_Contacts_ContactId")
            .IsUnique();

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<ContactEntity> entity);
}