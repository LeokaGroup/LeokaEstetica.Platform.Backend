using LeokaEstetica.Platform.Base.Abstractions.Repositories.User;
using LeokaEstetica.Platform.Core.Exceptions;
using LeokaEstetica.Platform.Database.Abstractions.ProjectManagment;
using LeokaEstetica.Platform.Models.Enums;
using LeokaEstetica.Platform.Notifications.Abstractions;
using LeokaEstetica.Platform.Notifications.Consts;
using LeokaEstetica.Platform.Services.Abstractions.ProjectManagment;
using LeokaEstetica.Platform.Services.Helpers;
using Microsoft.Extensions.Logging;

namespace LeokaEstetica.Platform.Services.Services.ProjectManagment;

/// <summary>
/// Класс реализует методы сервиса эпика.
/// </summary>
internal sealed class EpicService : IEpicService
{
    private readonly IEpicRepository _epicRepository;
    private readonly ILogger<EpicService> _logger;
    private readonly IUserRepository _userRepository;
    private readonly Lazy<IHubNotificationService> _hubNotificationService;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="epicRepository"></param>
    /// <param name="logger">Логгер.</param>
    /// <param name="userRepository">Репозиторий пользователей.</param>
    /// <param name="hubNotificationService">Сервис уведомлений хабов.</param>
    public EpicService(IEpicRepository epicRepository,
        ILogger<EpicService> logger,
        IUserRepository userRepository,
        Lazy<IHubNotificationService> hubNotificationService )
    {
        _epicRepository = epicRepository;
        _logger = logger;
        _userRepository = userRepository;
        _hubNotificationService = hubNotificationService;
    }

    /// <inheritdoc />
    public async Task ExcludeEpicTasksAsync(long epicId, IEnumerable<string>? projectTaskIds, string account)
    {
        var userId = await _userRepository.GetUserByEmailAsync(account);

        if (userId <= 0)
        {
            var ex = new NotFoundUserIdByAccountException(account);
            throw ex;
        }

        var userCode = await _userRepository.GetUserCodeByUserIdAsync(userId);

        try
        {
            await _epicRepository.ExcludeEpicTasksAsync(epicId,
                projectTaskIds!.Select(x => x.GetProjectTaskIdFromPrefixLink()));

            await _hubNotificationService.Value.SendNotificationAsync("Все хорошо",
               "Задача успешно исключена из эпика.",
               NotificationLevelConsts.NOTIFICATION_LEVEL_SUCCESS, "SendNotifySuccessExcludeEpicTask",
               userCode, UserConnectionModuleEnum.ProjectManagement);
        }
        
        catch (Exception ex)
        {
            _logger?.LogError(ex, ex.Message);

            await _hubNotificationService.Value.SendNotificationAsync("Что то пошло не так",
                "Ошибка при исключении задачи из эпика.",
                NotificationLevelConsts.NOTIFICATION_LEVEL_ERROR, "SendNotifyErrorExcludeEpicTask", userCode,
                UserConnectionModuleEnum.ProjectManagement);

            throw;
        }
    }
}