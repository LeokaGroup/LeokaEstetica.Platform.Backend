using LeokaEstetica.Platform.Database.Abstractions.User;

namespace LeokaEstetica.Platform.Services.Strategies.Project.Team;

/// <summary>
/// Базовый класс приглашений в команду проекта.
/// Базовая стратегия.
/// </summary>
public abstract class BaseProjectInviteTeamStrategy
{
    protected readonly IUserRepository UserRepository;
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="userRepository">Репозиторий пользователя.</param>
    protected BaseProjectInviteTeamStrategy(IUserRepository userRepository)
    {
        UserRepository = userRepository;
    }

    /// <summary>
    /// Метод находит Id пользователя по выбранной стратегии.
    /// </summary>
    /// <param name="inviteText">Текст, который будет использоваться для поиска пользователя для приглашения.</param>
    /// <returns>Id пользователя</returns>
    public abstract Task<long> GetUserId(string inviteText);
}