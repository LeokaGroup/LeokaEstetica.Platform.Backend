using LeokaEstetica.Platform.Core.Extensions;
using LeokaEstetica.Platform.Models.Entities.Common;
using LeokaEstetica.Platform.Models.Entities.Landing;
using LeokaEstetica.Platform.Models.Entities.Logs;
using LeokaEstetica.Platform.Models.Entities.User;
using Microsoft.EntityFrameworkCore;

namespace LeokaEstetica.Platform.Core.Data;

/// <summary>
/// Класс датаконтекста Postgres.
/// </summary>
public class PgContext : DbContext
{
    private readonly DbContextOptions<PgContext> _options;

    public PgContext(DbContextOptions<PgContext> options) 
        : base(options)
    {
        _options = options;
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Настраиваем все маппинги приложения.
        MappingsExtensions.Configure(modelBuilder);
    }
    
    /// <summary>
    /// Таблица фона.
    /// </summary>
    public DbSet<FonEntity> Fons { get; set; }

    /// <summary>
    /// Таблица логов.
    /// </summary>
    public DbSet<LogInfoEntity> LogInfos { get; set; }

    /// <summary>
    /// Таблица хидера.
    /// </summary>
    public DbSet<HeaderEntity> Header { get; set; }

    /// <summary>
    /// Таблица предложений платформы.
    /// </summary>
    public DbSet<PlatformOfferEntity> PlatformOffer { get; set; }

    /// <summary>
    /// Таблица элементов предложений платформы.
    /// </summary>
    public DbSet<PlatformOfferItemsEntity> PlatformOfferItems { get; set; }

    /// <summary>
    /// Таблица пользователей.
    /// </summary>
    public DbSet<UserEntity> Users { get; set; }
}