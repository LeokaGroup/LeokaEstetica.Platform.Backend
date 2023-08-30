using LeokaEstetica.Platform.Models.Entities.Communication;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeokaEstetica.Platform.Models.Mappings.Communication;

public partial class MainInfoDialogConfiguration : IEntityTypeConfiguration<MainInfoDialogEntity>
{
    public void Configure(EntityTypeBuilder<MainInfoDialogEntity> entity)
    {
        entity.ToTable("MainInfoDialogs", "Communications");

        entity.HasKey(e => e.DialogId);

        entity.Property(e => e.DialogId)
            .HasColumnName("DialogId")
            .HasColumnType("bigserial");
        
        entity.Property(e => e.DialogName)
            .HasColumnName("DialogName")
            .HasColumnType("varchar(150)")
            .IsRequired();
        
        entity.Property(e => e.Created)
            .HasColumnName("Created")
            .HasColumnType("timestamp")
            .IsRequired();

        entity.HasIndex(u => u.DialogId)
            .HasDatabaseName("PK_MainInfoDialogs_DialogId")
            .IsUnique();
        
        entity.HasOne(p => p.CatalogProject)
            .WithMany(b => b.MainInfoDialog)
            .HasForeignKey(p => p.ProjectId)
            .HasConstraintName("FK_CatalogProjects_ProjectId");

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<MainInfoDialogEntity> entity);
}