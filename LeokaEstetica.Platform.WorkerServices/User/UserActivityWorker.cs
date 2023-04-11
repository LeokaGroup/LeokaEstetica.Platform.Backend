namespace LeokaEstetica.Platform.WorkerServices.User;

/// <summary>
/// Класс воркер-сервиса активности пользователей.
/// </summary>
public class UserActivityWorker : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            
        }
    }
}