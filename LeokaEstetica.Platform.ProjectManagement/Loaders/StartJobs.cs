using LeokaEstetica.Platform.ProjectManagement.Loaders.Jobs;
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
    public static void Start(IServiceCollectionQuartzConfigurator q, IServiceCollection services)
    {
        var ordersJobJobKey = new JobKey("SprintDurationJob");
        q.AddJob<SprintDurationJob>(opts => opts.WithIdentity(ordersJobJobKey));
        q.AddTrigger(opts => opts
            .ForJob(ordersJobJobKey)
            .WithIdentity("SprintDurationJobTrigger")
            .WithSimpleSchedule(x => x
                // .WithIntervalInMinutes(3)
                .WithIntervalInSeconds(5)
                .RepeatForever()));
    }
}