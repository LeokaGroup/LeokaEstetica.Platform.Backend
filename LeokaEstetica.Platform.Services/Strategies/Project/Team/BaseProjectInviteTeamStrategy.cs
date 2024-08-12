using LeokaEstetica.Platform.Base.Abstractions.Repositories.User;
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
    protected readonly Lazy<IHubNotificationService> HubNotificationService;
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="userRepository">Репозиторий пользователя.</param>
    /// <param name="projectNotificationsService">Сервис уведомлений проектов.</param>
    /// <param name="hubNotificationService">Сервис уведомлений хабов.</param>
    protected BaseProjectInviteTeamStrategy(IUserRepository userRepository,
        IProjectNotificationsService projectNotificationsService,
        Lazy<IHubNotificationService> hubNotificationService)
    {
        UserRepository = userRepository;
        ProjectNotificationsService = projectNotificationsService;
        HubNotificationService = hubNotificationService;
    }

    /// <summary>
    /// Метод находит Id пользователя по выбранной стратегии.
    /// </summary>
    /// <param name="inviteText">Текст, который будет использоваться для поиска пользователя для приглашения.</param>
    /// <param name="account">Аккаунт.</param>
    /// <returns>Id пользователя</returns>
    public abstract Task<long> GetUserIdAsync(string inviteText, string? account);
}