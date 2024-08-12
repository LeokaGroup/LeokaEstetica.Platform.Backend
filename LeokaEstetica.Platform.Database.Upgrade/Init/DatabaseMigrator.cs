using System.Reflection;
using DbUp;

namespace LeokaEstetica.Platform.Database.Upgrade.Init;

/// <summary>
/// Класс инициализации миграций.
/// </summary>
public class DatabaseMigrator
{
    /// <summary>
    /// Метод накатывает миграции.
    /// </summary>
    /// <param name="connectionString">Строка подключения к БД.</param>
    public static void MigrateDatabase(string connectionString)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var assemblyName = assembly.GetName().Name;

        var executor = DeployChanges.To
            .PostgresqlDatabase(connectionString)
            .WithScriptsEmbeddedInAssembly(assembly,
                s => s.StartsWith(string.Join('.', assemblyName, "Scripts"),
                    StringComparison.InvariantCultureIgnoreCase))
            .JournalToPostgresqlTable("public", "schemaversions")
            .WithVariablesDisabled()
            .LogToConsole()
            .Build();

        var result = executor.PerformUpgrade();

        if (!result.Successful)
        {
            Console.WriteLine("Ошибка при обновлении БД.");
            Console.WriteLine(result.Error);
        }
        
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Успешно!");
        Console.ResetColor();
    }
}