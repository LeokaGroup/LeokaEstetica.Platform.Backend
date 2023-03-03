using LeokaEstetica.Platform.Database.Abstractions.User;
using LeokaEstetica.Platform.Notifications.Data;
using Microsoft.AspNetCore.SignalR;

namespace LeokaEstetica.Platform.Services.Strategies.Project.Team;

/// <summary>
/// Базовый класс приглашений в команду проекта.
/// Базовая стратегия.
/// </summary>
public abstract class BaseProjectInviteTeamStrategy
{
    protected readonly IUserRepository UserRepository;
    protected readonly IHubContext<NotifyHub> _hubContext;
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="userRepository">Репозиторий пользователя.</param>
    protected BaseProjectInviteTeamStrategy(IUserRepository userRepository, 
        IHubContext<NotifyHub> hubContext)
    {
        UserRepository = userRepository;
        _hubContext = hubContext;
    }

    /// <summary>
    /// Метод находит Id пользователя по выбранной стратегии.
    /// </summary>
    /// <param name="inviteText">Текст, который будет использоваться для поиска пользователя для приглашения.</param>
    /// <returns>Id пользователя</returns>
    public abstract Task<long> GetUserId(string inviteText);
}