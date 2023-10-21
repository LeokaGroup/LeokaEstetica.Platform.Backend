using LeokaEstetica.Platform.Database.Upgrade.Init;
using Microsoft.Extensions.Configuration;

var builder = new ConfigurationBuilder();
builder.SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

IConfiguration configuration = builder.Build();

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