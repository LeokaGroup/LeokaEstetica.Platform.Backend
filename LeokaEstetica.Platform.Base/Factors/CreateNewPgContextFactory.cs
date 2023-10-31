using LeokaEstetica.Platform.Core.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace LeokaEstetica.Platform.Base.Factors;

/// <summary>
/// Класс факторки создает новый PgContext.
/// </summary>
public static class CreateNewPgContextFactory
{
    /// <summary>
    /// Метод создает новый PgContext.
    /// </summary>
    /// <param name="configuration">Конфигурация приложения.</param>
    /// <returns>PgContext.</returns>
    public static PgContext CreateNewPgContext(IConfiguration configuration)
    {
        string connectionType = null;
        
        if (configuration["Environment"].Equals("Development"))
        {
            connectionType = "NpgDevSqlConnection";
        }
        
        else if (configuration["Environment"].Equals("Staging"))
        {
            connectionType = "NpgTestSqlConnection";
        }
        
        else if (configuration["Environment"].Equals("Production"))
        {
            connectionType = "NpgSqlConnection";
        }
        
        var optionsBuilder = new DbContextOptionsBuilder<PgContext>();
        optionsBuilder.UseNpgsql(configuration.GetConnectionString(connectionType));
        var pgContext = new PgContext(optionsBuilder.Options);

        return pgContext;
    }
}