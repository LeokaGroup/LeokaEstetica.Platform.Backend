using LeokaEstetica.Platform.WorkerServices.Jobs.RabbitMq;
using Quartz;

namespace LeokaEstetica.Platform.Backend.Loaders.Jobs;

/// <summary>
/// Класс с джобами, которые должны выполняться при запуске ядра системы.
/// </summary>
public static class StartJobs
{
    /// <summary>
    /// Метод запускает все джобы.
    /// </summary>
    public static void Start(IServiceCollectionQuartzConfigurator q)
    {
        // Запускаем планировщик активностей аккаунтов пользователей.
        //services.AddHostedService<UserActivityMarkDeactivateJob>();

        // Запускаем планировщик удаления аккаунтов пользователей.
        //services.AddHostedService<DeleteDeactivatedAccountsJob>();
        //services.AddHostedService<OrdersJob>();

        var refundsJobJobKey = new JobKey("RefundsJob");
        q.AddJob<RefundsJob>(opts => opts.WithIdentity(refundsJobJobKey));
        q.AddTrigger(opts => opts
            .ForJob(refundsJobJobKey)
            .WithIdentity("RefundsJobTrigger")
            .WithSimpleSchedule(x => x
                .WithIntervalInMinutes(3)
                .RepeatForever()));
        
        var ordersJobJobKey = new JobKey("OrdersJob");
        q.AddJob<OrdersJob>(opts => opts.WithIdentity(ordersJobJobKey));
        q.AddTrigger(opts => opts
            .ForJob(ordersJobJobKey)
            .WithIdentity("OrdersJobTrigger")
            .WithSimpleSchedule(x => x
                .WithIntervalInMinutes(3)
                .RepeatForever()));
    }
}