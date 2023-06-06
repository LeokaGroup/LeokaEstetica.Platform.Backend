using LeokaEstetica.Platform.Database.Abstractions.User;
using LeokaEstetica.Platform.Notifications.Abstractions;
using LeokaEstetica.Platform.Notifications.Consts;

namespace LeokaEstetica.Platform.Services.Strategies.Project.Team;

/// <summary>
/// Стратегия приглашения пользователя в проект по Email.
/// </summary>
internal class ProjectInviteTeamEmailStrategy : BaseProjectInviteTeamStrategy
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="userRepository">Репозиторий пользователя.</param>
    public ProjectInviteTeamEmailStrategy(IUserRepository userRepository,
        IProjectNotificationsService projectNotificationsService) : base(userRepository, projectNotificationsService)
    {
    }

    /// <summary>
    /// Метод находит Id пользователя по его Email.
    /// </summary>
    /// <param name="inviteText">Поисковый параметр.</param>
    /// <param name="token">Токен пользователя.></param>
    /// <returns>Токен пользователя.</returns>
    public override async Task<long> GetUserId(string inviteText, string token)
    {
        var result = await UserRepository.GetUserIdByEmailAsync(inviteText);
        
        if (result <= 0)
        {
            await ProjectNotificationsService.SendNotificationErrorProjectInviteTeamByEmailAsync(
                "Внимание",
                "Не удалось пригласить пользователя по Email. Проверьте корректность Email пользователя.",
                NotificationLevelConsts.NOTIFICATION_LEVEL_WARNING, token);
        }

        return result;
    }
}