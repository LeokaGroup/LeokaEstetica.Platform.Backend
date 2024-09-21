using Quartz;

namespace LeokaEstetica.Platform.ProjectManagement.HumanResources.Loaders;

/// <summary>
/// Класс с джобами, которые должны выполняться при запуске модуля HR.
/// </summary>
public static class StartJobs
{
     /// <summary>
    /// Метод запускает все джобы модуля HR.
    /// <param name="q">Конфигуратор Quartz.</param>
    /// <param name="configuration">Конфигурация приложения.</param>
    /// </summary>
    public static void Start(IServiceCollectionQuartzConfigurator q, IConfiguration configuration)
    {
       
    }
}