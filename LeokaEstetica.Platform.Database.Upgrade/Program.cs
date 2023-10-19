using LeokaEstetica.Platform.Database.Upgrade.Init;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions());
var configuration = builder.Configuration;

builder.Environment.EnvironmentName = configuration["Environment"];

if (builder.Environment.IsDevelopment())
{
    DatabaseMigrator.MigrateDatabase(configuration["ConnectionStrings:NpgDevSqlConnection"]);
}

if (builder.Environment.IsStaging())
{
    DatabaseMigrator.MigrateDatabase(configuration["ConnectionStrings:NpgTestSqlConnection"]);
}

if (builder.Environment.IsProduction())
{
    DatabaseMigrator.MigrateDatabase(configuration["ConnectionStrings:NpgSqlConnection"]);
}

var app = builder.Build();

app.Run();