using LeokaEstetica.Platform.Database.Abstractions.User;
using LeokaEstetica.Platform.Logs.Abstractions;
using LeokaEstetica.Platform.Redis.Abstractions.User;
using LeokaEstetica.Platform.Redis.Consts;
using LeokaEstetica.Platform.Services.Abstractions.Project;

namespace LeokaEstetica.Platform.WorkerServices.Jobs.User;

/// <summary>
/// Класс джобы удаления аккаунтов пользователей и всех связанных данных.
/// </summary>
public class DeleteDeactivatedAccountsJob : BackgroundService
{
    private Timer _timer;
    private readonly ILogService _logService;
    private readonly IUserRedisService _userRedisService;
    private readonly IProjectService _projectService;
    private readonly IUserRepository _userRepository;

    /// <summary>
    /// Конструктор.
    /// <param name="Сервис логов."></param>
    /// <param name="userRedisService">Сервис кэша.</param>
    /// </summary>
    public DeleteDeactivatedAccountsJob(ILogService logService,
        IUserRedisService userRedisService,
        IProjectService projectService,
        IUserRepository userRepository)
    {
        _logService = logService;
        _userRedisService = userRedisService;
        _projectService = projectService;
        _userRepository = userRepository;
    }

    /// <summary>
    /// Метод выполняет логику фоновой задачи.
    /// </summary>
    /// <param name="IJobExecutionContext">Контекст джобы.</param>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(30)); // TODO: После тестов на 24 ч.

        await Task.CompletedTask;
    }

    /// <summary>
    /// Метод выполняет логику фоновой задачи.
    /// </summary>
    /// <param name="state">Состояние.</param>
    private async void DoWork(object state)
    {
        try
        {
            var deleteUsers = await _userRedisService.GetMarkDeactivateUserAccountsAsync(
                CacheKeysConsts.DEACTIVATE_USER_ACCOUNTS);

            if (!deleteUsers.Any())
            {
                await Task.CompletedTask;
            }

            foreach (var u in deleteUsers)
            {
                var userProjects = await _projectService.UserProjectsAsync(u.Email);

                // Пропускем, если нет проектов для удаления.
                if (!userProjects.UserProjects.Any())
                {
                    continue;
                }

                // Перебираем все проекты для удаления.
                foreach (var p in userProjects.UserProjects)
                {
                    // Удаляем проект и все связанное с ним.
                    // Токен не передаем, так как уведомления показывать не надо в этом кейсе.
                    await _projectService.DeleteProjectAsync(p.ProjectId, u.Email, null);
                }
            }

            // Удаляем все аккаунты.
            await _userRepository.DeleteDeactivateAccountsAsync(deleteUsers);
        }

        catch (Exception ex)
        {
            await _logService.LogCriticalAsync(ex, "Ошибка при выполнении фоновой задачи DeleteDeactivatedAccounts.");
            throw;
        }
    }

    /// <summary>
    /// Метод останавливает фоновую задачу.
    /// </summary>
    /// <param name="stoppingToken">Токен остановки.</param>
    public override Task StopAsync(CancellationToken stoppingToken)
    {
        _timer?.Change(Timeout.Infinite, 0);

        return Task.CompletedTask;
    }

    /// <summary>
    /// Метод освобождает ресурсы таймера.
    /// </summary>
    public override void Dispose()
    {
        _timer?.Dispose();
    }
}