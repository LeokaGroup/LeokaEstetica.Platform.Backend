using LeokaEstetica.Platform.Database.Abstractions.User;
using LeokaEstetica.Platform.Notifications.Abstractions;

namespace LeokaEstetica.Platform.Services.Strategies.Project.Team;

/// <summary>
/// Базовый класс приглашений в команду проекта.
/// Базовая стратегия.
/// </summary>
internal abstract class BaseProjectInviteTeamStrategy
{
    protected readonly IUserRepository UserRepository;
    protected readonly IProjectNotificationsService ProjectNotificationsService;
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="userRepository">Репозиторий пользователя.</param>
    /// <param name="projectNotificationsService">Сервис уведомлений проектов.</param>
    protected BaseProjectInviteTeamStrategy(IUserRepository userRepository, 
        IProjectNotificationsService projectNotificationsService)
    {
        UserRepository = userRepository;
        ProjectNotificationsService = projectNotificationsService;
    }

    /// <summary>
    /// Метод находит Id пользователя по выбранной стратегии.
    /// </summary>
    /// <param name="inviteText">Текст, который будет использоваться для поиска пользователя для приглашения.</param>
    /// <param name="Токен пользователя."></param>
    /// <returns>Id пользователя</returns>
    public abstract Task<long> GetUserId(string inviteText, string token);
}