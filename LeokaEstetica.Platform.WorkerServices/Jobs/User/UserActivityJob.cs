using Quartz;

namespace LeokaEstetica.Platform.WorkerServices.Jobs.User;

/// <summary>
/// Класс джобы активности пользователей.
/// </summary>
public class UserActivityJob : IJob
{
    /// <summary>
    /// Метод выполняет логику фоновой задачи.
    /// </summary>
    /// <param name="stoppingToken">Токен завершения.</param>
    public async Task Execute(IJobExecutionContext context)
    {
        Console.WriteLine("UserActivityWorker is starting...");
    }
}