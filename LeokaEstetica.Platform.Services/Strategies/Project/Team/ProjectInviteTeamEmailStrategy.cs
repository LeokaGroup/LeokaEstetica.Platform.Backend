using LeokaEstetica.Platform.Database.Abstractions.User;
using LeokaEstetica.Platform.Notifications.Data;
using Microsoft.AspNetCore.SignalR;

namespace LeokaEstetica.Platform.Services.Strategies.Project.Team;

/// <summary>
/// Стратегия приглашения пользователя в проект по Email.
/// </summary>
public class ProjectInviteTeamEmailStrategy : BaseProjectInviteTeamStrategy
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="userRepository">Репозиторий пользователя.</param>
    public ProjectInviteTeamEmailStrategy(IUserRepository userRepository,
        IHubContext<NotifyHub> hubContext) : base(userRepository, hubContext)
    {
    }

    /// <summary>
    /// Метод находит Id пользователя по его Email.
    /// </summary>
    /// <param name="inviteText">Поисковый параметр.</param>
    /// <returns>Id пользователя.</returns>
    public override async Task<long> GetUserId(string inviteText)
    {
        var result = await UserRepository.GetUserIdByEmailAsync(inviteText);

        return result;
    }
}