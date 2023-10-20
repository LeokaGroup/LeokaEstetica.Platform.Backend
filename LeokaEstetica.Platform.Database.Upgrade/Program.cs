using LeokaEstetica.Platform.Database.Upgrade.Init;

// var builder = WebApplication.CreateBuilder(new WebApplicationOptions());
// var configuration = builder.Configuration;

// builder.Environment.EnvironmentName = configuration["Environment"];

// builder.Services.AddTransient<DatabaseMigrator>();

DatabaseMigrator.MigrateDatabase("User ID=chucknorris;Password=G3t7nQqbCyjuT8W;Server=80.78.251.69;Port=5432;Database=leoka_estetica_dev;Integrated Security=true;Pooling=true");

// if (builder.Environment.IsDevelopment())
// {
//     DatabaseMigrator.MigrateDatabase(configuration["ConnectionStrings:NpgDevSqlConnection"]);
// }

// if (builder.Environment.IsStaging())
// {
//     DatabaseMigrator.MigrateDatabase(configuration["ConnectionStrings:NpgTestSqlConnection"]);
// }
//
// if (builder.Environment.IsProduction())
// {
//     DatabaseMigrator.MigrateDatabase(configuration["ConnectionStrings:NpgSqlConnection"]);
// }