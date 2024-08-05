using LeokaEstetica.Platform.Core.Enums;
using LeokaEstetica.Platform.Notifications.Abstractions;
using LeokaEstetica.Platform.Notifications.Data;
using LeokaEstetica.Platform.Notifications.Models.Output;
using LeokaEstetica.Platform.Redis.Enums;
using Microsoft.AspNetCore.SignalR;

namespace LeokaEstetica.Platform.Notifications.Services;

/// <summary>
/// Класс реализует методы сервиса уведомлений хабов.
/// </summary>
internal sealed class HubNotificationService : IHubNotificationService
{
    private readonly IHubContext<ChatHub> _mainHubContext;
    private readonly IHubContext<ProjectManagementHub> _projectManagementHub;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="mainHubContext">Хаб основного модуля.</param>
    /// <param name="projectManagementHub">Хаб модуля УП.</param>
    public HubNotificationService(IHubContext<ChatHub> mainHubContext,
        IHubContext<ProjectManagementHub> projectManagementHub)
    {
        _mainHubContext = mainHubContext;
        _projectManagementHub = projectManagementHub;
    }

    /// <inheritdoc />
    public async Task SendNotificationAsync(string title, string notifyText, string notificationLevel, string function,
        Guid userCode, UserConnectionModuleEnum module)
    {
        var key = userCode + "_" + module;

        switch (module)
        {
            // Отправляем уведомления в хаб основного модуля.
            case UserConnectionModuleEnum.Main:
                await _mainHubContext.Clients.Client(key).SendAsync(function, new NotificationOutput
                {
                    Title = title,
                    NotificationLevel = notificationLevel,
                    Message = notifyText
                });
                break;

            // Отправляем уведомления в хаб модуля УП.
            case UserConnectionModuleEnum.ProjectManagement:
                await _projectManagementHub.Clients.Client(key).SendAsync(function, new NotificationOutput
                {
                    Title = title,
                    NotificationLevel = notificationLevel,
                    Message = notifyText
                });
                break;

            default:
                throw new InvalidOperationException("Неизвестный тип модуля приложения для уведомлений хабов.");
        }
    }

    /// <inheritdoc />
    public async Task SendClassificationNetworkMessageResultAsync(string message, string connectionId, long dialogId)
    {
        await _projectManagementHub.Clients
            .Client(connectionId)
            .SendAsync("SendClassificationNetworkMessageResult", new ClassificationNetworkMessageOutput
            {
                Response = message,
                ConnectionId = connectionId,
                IsMyMessage = true,
                ScrumMasterAiEventType = ScrumMasterAiEventTypeEnum.Message.ToString(),
                DialogId = dialogId
            });
    }
}