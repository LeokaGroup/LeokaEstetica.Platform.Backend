using LeokaEstetica.Platform.Database.Abstractions.User;

namespace LeokaEstetica.Platform.Services.Strategies.Project.Team;

/// <summary>
/// Стратегия приглашения пользователя в проект по номеру телефона.
/// </summary>
public class ProjectInviteTeamPhoneNumberStrategy : BaseProjectInviteTeamStrategy
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="userRepository">Репозиторий пользователя.</param>
    public ProjectInviteTeamPhoneNumberStrategy(IUserRepository userRepository) : base(userRepository)
    {
    }

    /// <summary>
    /// Метод находит Id пользователя по его Email.
    /// </summary>
    /// <param name="inviteText">Поисковый параметр.</param>
    /// <returns>Id пользователя.</returns>
    public override async Task<long> GetUserId(string inviteText)
    {
        var result = await UserRepository.GetUserIdByPhoneNumberAsync(inviteText);

        return result;
    }
}