using System.Reflection;
using DbUp;

namespace LeokaEstetica.Platform.Database.Upgrade.Init;

public static class DatabaseMigrator
{
    public static void MigrateDatabase(string connectionString)
    {
        EnsureDatabase.For.PostgresqlDatabase(connectionString);

        var upgrader = DeployChanges.To
            .PostgresqlDatabase(connectionString)
            .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
            .WithTransaction()
            .LogToConsole()
            .Build();

        var result = upgrader.PerformUpgrade();

        if (!result.Successful)
        {
            throw new Exception("Ошибка при обновлении БД.", result.Error);
        }
    }
}