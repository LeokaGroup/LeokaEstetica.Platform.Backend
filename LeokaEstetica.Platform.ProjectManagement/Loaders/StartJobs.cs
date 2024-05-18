using LeokaEstetica.Platform.ProjectManagement.Loaders.Jobs;
using LeokaEstetica.Platform.ProjectManagement.ScrumMasterAI.Jobs.RabbitMq;
using Quartz;

namespace LeokaEstetica.Platform.ProjectManagement.Loaders;

/// <summary>
/// Класс с джобами, которые должны выполняться при запуске модуля УП.
/// </summary>
public static class StartJobs
{
    /// <summary>
    /// Метод запускает все джобы модуля УП.
    /// </summary>
    public static void Start(IServiceCollectionQuartzConfigurator q)
    {
        var ordersJobKey = new JobKey("SprintDurationJob");
        q.AddJob<SprintDurationJob>(opts => opts.WithIdentity(ordersJobKey));
        q.AddTrigger(opts => opts
            .ForJob(ordersJobKey)
            .WithIdentity("SprintDurationJobTrigger")
            .WithSimpleSchedule(x => x
                .WithIntervalInMinutes(3)
                .RepeatForever()));
                
        var scrumMasterAiJobKey = new JobKey("ScrumMasterAiJobKey");
        q.AddJob<ScrumMasterAIJob>(opts => opts.WithIdentity(scrumMasterAiJobKey));
        q.AddTrigger(opts => opts
            .ForJob(scrumMasterAiJobKey)
            .WithIdentity("ScrumMasterAiJobKeyTrigger")
            .WithSimpleSchedule(x => x
                .WithIntervalInSeconds(3)
                .RepeatForever()));
    }
}