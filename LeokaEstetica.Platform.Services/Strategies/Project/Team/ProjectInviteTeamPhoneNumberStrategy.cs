using LeokaEstetica.Platform.Database.Abstractions.User;
using LeokaEstetica.Platform.Notifications.Abstractions;
using LeokaEstetica.Platform.Notifications.Consts;

namespace LeokaEstetica.Platform.Services.Strategies.Project.Team;

/// <summary>
/// Стратегия приглашения пользователя в проект по номеру телефона.
/// </summary>
internal class ProjectInviteTeamPhoneNumberStrategy : BaseProjectInviteTeamStrategy
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="userRepository">Репозиторий пользователя.</param>
    public ProjectInviteTeamPhoneNumberStrategy(IUserRepository userRepository,
        IProjectNotificationsService projectNotificationsService) : base(userRepository, projectNotificationsService)
    {
    }

    /// <summary>
    /// Метод находит Id пользователя по его Email.
    /// </summary>
    /// <param name="inviteText">Поисковый параметр.</param>
    /// <returns>Id пользователя.</returns>
    public override async Task<long> GetUserId(string inviteText, string token)
    {
        var result = await UserRepository.GetUserIdByPhoneNumberAsync(inviteText);
        
        if (result <= 0)
        {
            await ProjectNotificationsService.SendNotificationErrorProjectInviteTeamByPhoneNumberAsync("Внимание",
                "Не удалось пригласить пользователя по номеру телефона. " +
                "Проверьте корректность номера телефона пользователя.",
                NotificationLevelConsts.NOTIFICATION_LEVEL_WARNING, token);
        }

        return result;
    }
}