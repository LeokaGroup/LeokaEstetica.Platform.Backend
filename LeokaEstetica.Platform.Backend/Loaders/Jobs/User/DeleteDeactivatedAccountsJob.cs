using LeokaEstetica.Platform.Base.Abstractions.Repositories.User;
using LeokaEstetica.Platform.Base.Abstractions.Services.Pachca;
using LeokaEstetica.Platform.Database.Abstractions.Moderation.Resume;
using LeokaEstetica.Platform.Database.Abstractions.Resume;
using LeokaEstetica.Platform.Database.Abstractions.Ticket;
using LeokaEstetica.Platform.Redis.Abstractions.User;
using LeokaEstetica.Platform.Redis.Consts;
using LeokaEstetica.Platform.Services.Abstractions.Project;
using LeokaEstetica.Platform.Services.Abstractions.Vacancy;
using Newtonsoft.Json;

namespace LeokaEstetica.Platform.Backend.Loaders.Jobs.User;

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
    private readonly IResumeRepository _resumeRepository;
    private readonly IResumeModerationRepository _resumeModerationRepository;
    private readonly IPachcaService _pachcaService;
    private readonly ITicketRepository _ticketRepository;

    /// <summary>
    /// Конструктор.
    /// <param name="logger">Сервис логов.</param>
    /// <param name="userRedisService">Сервис кэша.</param>
    /// <param name="projectService">Сервис проектов.</param>
    /// <param name="userRepository">Репозиторий пользователей.</param>
    /// <param name="vacancyService">Репозиторий вакансий.</param>
    /// <param name="resumeRepository">Репозиторий анкет.</param>
    /// <param name="resumeModerationRepository">Репозиторий модерации анкет.</param>
    /// <param name="pachcaService">Сервис пачки.</param>
    /// <param name="ticketRepository">Репозиторий тикетов.</param>
    /// </summary>
    public DeleteDeactivatedAccountsJob(ILogger<DeleteDeactivatedAccountsJob> logger,
        IUserRedisService userRedisService,
        IProjectService projectService,
        IUserRepository userRepository, 
        IVacancyService vacancyService,
        IResumeRepository resumeRepository,
        IResumeModerationRepository resumeModerationRepository,
        IPachcaService pachcaService,
        ITicketRepository ticketRepository)
    {
        _logger = logger;
        _userRedisService = userRedisService;
        _projectService = projectService;
        _userRepository = userRepository;
        _vacancyService = vacancyService;
        _resumeRepository = resumeRepository;
        _resumeModerationRepository = resumeModerationRepository;
        _pachcaService = pachcaService;
        _ticketRepository = ticketRepository;
    }

    /// <summary>
    /// Метод выполняет логику фоновой задачи.
    /// </summary>
    /// <param name="stoppingToken">Токен отмены.</param>
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
                
                var userProjects = await _projectService.UserProjectsAsync(u.Email, false);
                
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
                        _logger.LogInformation("Начали удаление вакансий пользователей.");
                        
                        foreach (var v in vacancies.Vacancies)
                        {
                            await _vacancyService.DeleteVacancyAsync(v.VacancyId, u.Email, null);
                        }
                    }
                }
            }

            if (deleteUsers.Any())
            {
                var deleteUsersIds = deleteUsers.Select(u => u.UserId).ToList();
                
                // Находим анкеты пользователей.
                var resumes = await _resumeRepository.GetResumesAsync(deleteUsersIds);
                var profileItems = resumes.ToList();

                if (!profileItems.Any())
                {
                    throw new InvalidOperationException("Не удалось получить анкеты пользователей:" +
                                                        "Id пользователей к удалению:" +
                                                        $" {JsonConvert.SerializeObject(deleteUsersIds)}");
                }
                
                // Находим анкеты пользователей на модерации, которые сначала нужно удалить.
                var moderationResumesItems = await _resumeModerationRepository
                    .GetResumesModerationByProfileInfosIdsAsync(profileItems.Select(r => r.ProfileInfoId));
                var moderationResumes = moderationResumesItems.ToList();
                        
                if (!moderationResumes.Any())
                {
                    throw new InvalidOperationException("Не удалось получить анкеты пользователей на модерации:" +
                                                        "Анкеты пользователей к удалению:" +
                                                        $" {JsonConvert.SerializeObject(profileItems)}");
                }
                
                // Находим тикеты с пользователями, которые сначала нужно удалить.
                var ticketsMembers = await _ticketRepository.GetTicketsMembersByUserIdsAsync(deleteUsersIds);
                var ticketsMembersItems = ticketsMembers.ToList();

                if (!ticketsMembersItems.Any())
                {
                    throw new InvalidOperationException("Не удалось получить участников тикетов:" +
                                                        "Id пользователей к удалению:" +
                                                        $" {JsonConvert.SerializeObject(deleteUsersIds)}");
                }

                var ticketsMembersIds = ticketsMembersItems.Select(tm => tm.TicketId).ToList();
                
                // Находим сообщения тикетов, которые сначала нужно удалить.
                var ticketsMessages = await _ticketRepository.GetTicketsMessagesAsync(ticketsMembersIds);
                var ticketsMessagesItems = ticketsMessages.ToList();

                if (!ticketsMessagesItems.Any())
                {
                    throw new InvalidOperationException("Не удалось получить сообщения тикетов:" +
                                                        "Id участников тикетов с сообщениями к удалению:" +
                                                        $" {JsonConvert.SerializeObject(ticketsMembersIds)}");
                }

                // Удаляем все аккаунты.
                await _userRepository.DeleteDeactivateAccountsAsync(deleteUsers, profileItems, moderationResumes,
                    ticketsMembersItems, ticketsMessagesItems);   
                    
                // Удаляем пользователей из кэша, так как их мы уже удалили из БД.
                await _userRedisService.DeleteMarkDeactivateUserAccountsAsync();
            }
            
            _logger.LogInformation("Отработала джоба DeleteDeactivatedAccountsJob.");
        }

        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Ошибка при выполнении фоновой задачи DeleteDeactivatedAccounts.");
            
            await _pachcaService.SendNotificationErrorAsync(ex);
            
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