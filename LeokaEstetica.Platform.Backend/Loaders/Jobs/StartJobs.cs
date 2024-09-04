using LeokaEstetica.Platform.Backend.Loaders.Jobs.RabbitMq;
using Quartz;

namespace LeokaEstetica.Platform.Backend.Loaders.Jobs;

/// <summary>
/// Класс с джобами, которые должны выполняться при запуске ядра системы.
/// </summary>
public static class StartJobs
{
    /// <summary>
    /// Метод запускает все джобы.
    /// <param name="q">Конфигуратор Quartz.</param>
    /// <param name="services">Зарегистрированные сервисы DI.</param>
    /// <param name="configuration">Конфигурация приложения.</param>
    /// </summary>
    public static void Start(IServiceCollectionQuartzConfigurator q, IServiceCollection services,
        IConfiguration configuration)
    {
        // Запускаем планировщик активностей аккаунтов пользователей.
        //services.AddHostedService<UserActivityMarkDeactivateJob>();

        // Запускаем планировщик удаления аккаунтов пользователей.
        //services.AddHostedService<DeleteDeactivatedAccountsJob>();

        // TODO: Возвраты пока работают вручную (пока не автоматизируем их).
        // var refundsJobJobKey = new JobKey("RefundsJob");
        // q.AddJob<RefundsJob>(opts => opts.WithIdentity(refundsJobJobKey));
        // q.AddTrigger(opts => opts
        //     .ForJob(refundsJobJobKey)
        //     .WithIdentity("RefundsJobTrigger")
        //     .WithSimpleSchedule(x => x
        //         .WithIntervalInMinutes(3)
        //         .RepeatForever()));

        var ordersJobJobKey = new JobKey("OrdersJob");
        q.AddJob<OrdersJob>(opts =>
        {
            opts.WithIdentity(ordersJobJobKey);
            opts.UsingJobData("RabbitMq:HostName", configuration["RabbitMq:HostName"]!)
                .UsingJobData("RabbitMq:VirtualHost", configuration["RabbitMq:VirtualHost"]!)
                .UsingJobData("RabbitMq:UserName", configuration["RabbitMq:UserName"]!)
                .UsingJobData("RabbitMq:Password", configuration["RabbitMq:Password"]!)
                .UsingJobData("Environment", configuration["Environment"]!);
        });
        q.AddTrigger(opts => opts
            .ForJob(ordersJobJobKey)
            .WithIdentity("ScrumMasterAiJobKeyTrigger")
            .WithIdentity("OrdersJobTrigger")
            .WithSimpleSchedule(x => x
                // .WithIntervalInMinutes(3)
                .WithIntervalInSeconds(3)
                .RepeatForever()));
    }
}