using LeokaEstetica.Platform.Base.Abstractions.Repositories.User;
using LeokaEstetica.Platform.Core.Exceptions;
using LeokaEstetica.Platform.Models.Enums;
using LeokaEstetica.Platform.Notifications.Abstractions;
using LeokaEstetica.Platform.Notifications.Consts;

namespace LeokaEstetica.Platform.Services.Strategies.Project.Team;

/// <summary>
/// Стратегия приглашения пользователя в проект по Email.
/// </summary>
internal sealed class ProjectInviteTeamEmailStrategy : BaseProjectInviteTeamStrategy
{
    /// <inheritdoc />
    public ProjectInviteTeamEmailStrategy(IUserRepository userRepository,
        IProjectNotificationsService projectNotificationsService,
        Lazy<IHubNotificationService> hubNotificationService)
        : base(userRepository, projectNotificationsService, hubNotificationService)
    {
    }

    /// <inheritdoc />
    public override async Task<long> GetUserIdAsync(string inviteText, string? account)
    {
        var result = await UserRepository.GetUserIdByEmailAsync(inviteText);
        
        if (result <= 0)
        {
            var userId = await UserRepository.GetUserByEmailAsync(account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                throw ex;
            }
            
            var userCode = await UserRepository.GetUserCodeByUserIdAsync(userId);

            await HubNotificationService.Value.SendNotificationAsync("Внимание",
                "Не удалось пригласить пользователя по Email. Проверьте корректность Email пользователя.",
                NotificationLevelConsts.NOTIFICATION_LEVEL_WARNING, "SendNotificationErrorProjectInviteTeamByEmail",
                userCode, UserConnectionModuleEnum.ProjectManagement);
        }

        return result;
    }
}