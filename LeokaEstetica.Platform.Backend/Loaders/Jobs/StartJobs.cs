using LeokaEstetica.Platform.WorkerServices.Jobs.RabbitMq;
using LeokaEstetica.Platform.WorkerServices.Jobs.User;

namespace LeokaEstetica.Platform.Backend.Loaders.Jobs;

/// <summary>
/// Класс с джобами, которые должны выполняться при запуске ядра системы.
/// </summary>
public static class StartJobs
{
    /// <summary>
    /// Метод запускает джобы.
    /// По дефолту все воркер сервисы зареганы как сингтон.
    /// </summary>
    public static void Start(IServiceCollection services)
    {
        // Запускаем планировщик активностей аккаунтов пользователей.
        //services.AddHostedService<UserActivityMarkDeactivateJob>();
        
        // Запускаем планировщик удаления аккаунтов пользователей.
        //services.AddHostedService<DeleteDeactivatedAccountsJob>();
        services.AddHostedService<OrdersJob>();
    }
}