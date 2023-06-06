using LeokaEstetica.Platform.Database.Abstractions.User;
using LeokaEstetica.Platform.Notifications.Abstractions;
using LeokaEstetica.Platform.Notifications.Consts;

namespace LeokaEstetica.Platform.Services.Strategies.Project.Team;

/// <summary>
/// Стратегия приглашения пользователя в проект по ссылке.
/// </summary>
internal class ProjectInviteTeamLinkStrategy : BaseProjectInviteTeamStrategy
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="userRepository">Репозиторий пользователя.</param>
    public ProjectInviteTeamLinkStrategy(IUserRepository userRepository,
        IProjectNotificationsService projectNotificationsService) : base(userRepository, projectNotificationsService)
    {
    }

    /// <summary>
    /// Метод находит Id пользователя по его коду.
    /// </summary>
    /// <param name="inviteText">Поисковый параметр.</param>
    /// <param name="token">Токен пользователя.</param>
    /// <returns>Id пользователя.</returns>
    public override async Task<long> GetUserId(string inviteText, string token)
    {
        var result = await UserRepository.GetUserIdByCodeAsync(inviteText);

        if (result <= 0)
        {
            await ProjectNotificationsService.SendNotificationErrorProjectInviteTeamByLinkAsync(
                "Внимание",
                "Не удалось пригласить пользователя по ссылке. Проверьте корректность ссылки.",
                NotificationLevelConsts.NOTIFICATION_LEVEL_WARNING, token);
        }

        return result;
    }
}