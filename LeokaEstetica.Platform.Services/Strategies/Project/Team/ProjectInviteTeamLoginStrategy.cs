using LeokaEstetica.Platform.Database.Abstractions.User;

namespace LeokaEstetica.Platform.Services.Strategies.Project.Team;

/// <summary>
/// Стратегия приглашения пользователя в проект по логину.
/// </summary>
public class ProjectInviteTeamLoginStrategy : BaseProjectInviteTeamStrategy
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="userRepository">Репозиторий пользователя.</param>
    public ProjectInviteTeamLoginStrategy(IUserRepository userRepository) : base(userRepository)
    {
    }

    /// <summary>
    /// Метод находит Id пользователя по его Email.
    /// </summary>
    /// <param name="inviteText">Поисковый параметр.</param>
    /// <returns>Id пользователя.</returns>
    public override async Task<long> GetUserId(string inviteText)
    {
        var result = await UserRepository.GetUserIdByLoginAsync(inviteText);

        return result;
    }
}