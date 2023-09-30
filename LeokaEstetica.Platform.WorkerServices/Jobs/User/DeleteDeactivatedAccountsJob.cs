using LeokaEstetica.Platform.Base.Abstractions.Repositories.User;
using LeokaEstetica.Platform.Redis.Abstractions.User;
using LeokaEstetica.Platform.Redis.Consts;
using LeokaEstetica.Platform.Services.Abstractions.Project;
using LeokaEstetica.Platform.Services.Abstractions.Vacancy;
using Newtonsoft.Json;

namespace LeokaEstetica.Platform.WorkerServices.Jobs.User;

/// <summary>
/// Класс джобы удаления аккаунтов пользователей и всех связанных данных.
/// </summary>
public class DeleteDeactivatedAccountsJob : BackgroundService
{
    private Timer _timer;
    private readonly ILogger<DeleteDeactivatedAccountsJob> _logger;
    private readonly IUserRedisService _userRedisService;
    private readonly IProjectService _projectService;
    private readonly IUserRepository _userRepository;
    private readonly IVacancyService _vacancyService;

    /// <summary>
    /// Конструктор.
    /// <param name="logger">Сервис логов.</param>
    /// <param name="userRedisService">Сервис кэша.</param>
    /// </summary>
    public DeleteDeactivatedAccountsJob(ILogger<DeleteDeactivatedAccountsJob> logger,
        IUserRedisService userRedisService,
        IProjectService projectService,
        IUserRepository userRepository, 
        IVacancyService vacancyService)
    {
        _logger = logger;
        _userRedisService = userRedisService;
        _projectService = projectService;
        _userRepository = userRepository;
        _vacancyService = vacancyService;
    }

    /// <summary>
    /// Метод выполняет логику фоновой задачи.
    /// </summary>
    /// <param name="IJobExecutionContext">Контекст джобы.</param>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromHours(24));

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
            _logger.LogInformation("Начало работы джобы DeleteDeactivatedAccountsJob.");
            
            var deleteUsers = await _userRedisService.GetMarkDeactivateUserAccountsAsync(
                CacheKeysConsts.DEACTIVATE_USER_ACCOUNTS);

            if (!deleteUsers.Any())
            {
                return;
            }

            foreach (var u in deleteUsers)
            {
                _logger.LogInformation(
                    $"Найденные пользователи для удаления: {JsonConvert.SerializeObject(deleteUsers)}");
                
                var userProjects = await _projectService.UserProjectsAsync(u.Email);
                
                // Пропускем, если нет проектов для удаления.
                if (userProjects.UserProjects.Any())
                {
                    _logger.LogInformation("Проектов для удаления нет.");
                
                    // Перебираем все проекты для удаления.
                    foreach (var p in userProjects.UserProjects)
                    {
                        // Удаляем проект и все связанное с ним.
                        // Токен не передаем, так как уведомления показывать не надо в этом кейсе.
                        await _projectService.DeleteProjectAsync(p.ProjectId, u.Email, null);
                    }
                }

                // Проектов то нет, но надо бы проверить вакансии пользователей, которые не были привязаны к проектам.
                else
                {
                    _logger.LogInformation("Проверяем вакансии пользователей, которые не были привязаны к проектам.");
                    
                    var vacancies = await _vacancyService.GetUserVacanciesAsync(u.Email);

                    if (vacancies.Vacancies.Any())
                    {
                        foreach (var v in vacancies.Vacancies)
                        {
                            await _vacancyService.DeleteVacancyAsync(v.VacancyId, u.Email, null);
                        }
                    }
                }
            }

            if (deleteUsers.Any())
            {
                // Удаляем все аккаунты.
                await _userRepository.DeleteDeactivateAccountsAsync(deleteUsers);   
            }
            
            _logger.LogInformation("Отработала джоба DeleteDeactivatedAccountsJob.");
        }

        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Ошибка при выполнении фоновой задачи DeleteDeactivatedAccounts.");
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