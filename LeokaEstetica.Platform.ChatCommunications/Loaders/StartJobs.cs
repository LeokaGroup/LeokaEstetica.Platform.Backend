using Quartz;

namespace LeokaEstetica.Platform.Communications.Loaders;

/// <summary>
/// Класс с джобами, которые должны выполняться при запуске модуля коммуникаций.
/// </summary>
public class StartJobs
{
     /// <summary>
    /// Метод запускает все джобы модуля УП.
    /// <param name="q">Конфигуратор Quartz.</param>
    /// <param name="configuration">Конфигурация приложения.</param>
    /// </summary>
    public static void Start(IServiceCollectionQuartzConfigurator q, IConfiguration configuration)
    {
        // var ordersJobKey = new JobKey("SprintDurationJob");
        // q.AddJob<SprintDurationJob>(opts =>
        // {
        //     opts.WithIdentity(ordersJobKey);
        //     opts.UsingJobData("RabbitMq:HostName", configuration["RabbitMq:HostName"]!)
        //         .UsingJobData("RabbitMq:VirtualHost", configuration["RabbitMq:VirtualHost"]!)
        //         .UsingJobData("RabbitMq:UserName", configuration["RabbitMq:UserName"]!)
        //         .UsingJobData("RabbitMq:Password", configuration["RabbitMq:Password"]!)
        //         .UsingJobData("Environment", configuration["Environment"]!);
        // });
        // q.AddTrigger(opts => opts
        //     .ForJob(ordersJobKey)
        //     .WithIdentity("SprintDurationJobTrigger")
        //     .WithSimpleSchedule(x => x
        //         .WithIntervalInMinutes(3)
        //         .RepeatForever()));
        //
        // var scrumMasterAiJobKey = new JobKey("ScrumMasterAiJobKey");
        // q.AddJob<ScrumMasterAIJob>(opts =>
        // {
        //     opts.WithIdentity(scrumMasterAiJobKey);
        //     opts.UsingJobData("RabbitMq:HostName", configuration["RabbitMq:HostName"]!)
        //         .UsingJobData("RabbitMq:VirtualHost", configuration["RabbitMq:VirtualHost"]!)
        //         .UsingJobData("RabbitMq:UserName", configuration["RabbitMq:UserName"]!)
        //         .UsingJobData("RabbitMq:Password", configuration["RabbitMq:Password"]!)
        //         .UsingJobData("Environment", configuration["Environment"]!);
        // });
        // q.AddTrigger(opts => opts
        //     .ForJob(scrumMasterAiJobKey)
        //     .WithIdentity("ScrumMasterAiJobKeyTrigger")
        //     .WithSimpleSchedule(x => x
        //         .WithIntervalInSeconds(3)
        //         .RepeatForever()));
    }
}