using LeokaEstetica.Platform.WorkerServices.User;

namespace LeokaEstetica.Platform.Backend.Loaders.HostedWorkerServices;

/// <summary>
/// Класс регистрации воркер-сервисов ядра системы.
/// </summary>
public static class HostedWorkerServices
{
    /// <summary>
    /// Метод добавляет в список зареганных сервисов воркер-сервисы.
    /// По дефолту все воркер сервисы зареганы как сингтон.
    /// </summary>
    /// <param name="services">Список сервисов приложения нужных для DI.</param>
    public static void AddHostedServices(IServiceCollection services)
    {
        services.AddHostedService<UserActivityWorker>();
    }
}