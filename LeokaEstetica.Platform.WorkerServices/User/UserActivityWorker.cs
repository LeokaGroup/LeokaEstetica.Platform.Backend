namespace LeokaEstetica.Platform.WorkerServices.User;

/// <summary>
/// Класс воркер-сервиса активности пользователей.
/// </summary>
public class UserActivityWorker : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Console.WriteLine("UserActivityWorker is starting...");
        
        // Если операция не отменена, то выполняем задержку в 200 миллисекунд.
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(200, stoppingToken);
        }

        await Task.CompletedTask;
    }
}