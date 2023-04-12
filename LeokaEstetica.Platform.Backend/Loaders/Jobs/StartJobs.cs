using LeokaEstetica.Platform.WorkerServices.Schedulers.User;

namespace LeokaEstetica.Platform.Backend.Loaders.Jobs;

/// <summary>
/// Класс с джобами, которые должны выполняться при запуске ядра системы.
/// </summary>
public class StartJobs
{
    /// <summary>
    /// Метод запускает джобы.
    /// По дефолту все воркер сервисы зареганы как сингтон.
    /// </summary>
    public static void Start()
    {
        // Джоба отслеживания активности пользователя раз в сутки.
        UserActivityScheduler.Start();
    }
}