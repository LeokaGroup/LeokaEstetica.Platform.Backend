using LeokaEstetica.Platform.Database.Upgrade.Init;
using Microsoft.Extensions.Configuration;

IConfiguration configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json",
        optional: false, reloadOnChange: true)
    .Build();

if (configuration["Environment"].Equals("Development"))
{
    DatabaseMigrator.MigrateDatabase(configuration["ConnectionStrings:NpgDevSqlConnection"]);
}

if (configuration["Environment"].Equals("Staging"))
{
    DatabaseMigrator.MigrateDatabase(configuration["ConnectionStrings:NpgTestSqlConnection"]);
}

if (configuration["Environment"].Equals("Production"))
{
    DatabaseMigrator.MigrateDatabase(configuration["ConnectionStrings:NpgSqlConnection"]);
}