using LeokaEstetica.Platform.Database.Abstractions.User;

namespace LeokaEstetica.Platform.Services.Strategies.Project.Team;

/// <summary>
/// Стратегия приглашения пользователя в проект по ссылке.
/// </summary>
public class ProjectInviteTeamLinkStrategy : BaseProjectInviteTeamStrategy
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="userRepository">Репозиторий пользователя.</param>
    public ProjectInviteTeamLinkStrategy(IUserRepository userRepository) : base(userRepository)
    {
    }

    /// <summary>
    /// Метод находит Id пользователя по его коду.
    /// </summary>
    /// <param name="inviteText">Поисковый параметр.</param>
    /// <returns>Id пользователя.</returns>
    public override async Task<long> GetUserId(string inviteText)
    {
        var result = await UserRepository.GetUserIdByCodeAsync(inviteText);

        return result;
    }
}