using LeokaEstetica.Platform.WorkerServices.User;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        // Воркер-сервис активности пользователей.
        services.AddHostedService<UserActivityWorker>();
    })
    .Build();

await host.RunAsync();