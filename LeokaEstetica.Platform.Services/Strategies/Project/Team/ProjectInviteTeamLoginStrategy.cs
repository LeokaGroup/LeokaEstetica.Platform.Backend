using LeokaEstetica.Platform.Database.Abstractions.User;
using LeokaEstetica.Platform.Notifications.Abstractions;
using LeokaEstetica.Platform.Notifications.Consts;

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
    public ProjectInviteTeamLoginStrategy(IUserRepository userRepository,
        IProjectNotificationsService projectNotificationsService) : base(userRepository, projectNotificationsService)
    {
    }

    /// <summary>
    /// Метод находит Id пользователя по его Email.
    /// </summary>
    /// <param name="inviteText">Поисковый параметр.</param>
    /// <param name="token">Токен пользователя.</param>
    /// <returns>Id пользователя.</returns>
    public override async Task<long> GetUserId(string inviteText, string token)
    {
        var result = await UserRepository.GetUserIdByLoginAsync(inviteText);
        
        if (result <= 0)
        {
            await ProjectNotificationsService.SendNotificationErrorProjectInviteTeamByLoginAsync("Внимание",
                "Не удалось пригласить пользователя по логину. " + "Проверьте корректность логина пользователя.", 
                NotificationLevelConsts.NOTIFICATION_LEVEL_WARNING, token);
        }

        return result;
    }
}