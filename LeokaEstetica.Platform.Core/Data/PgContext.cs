using LeokaEstetica.Platform.Core.Extensions;
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
}