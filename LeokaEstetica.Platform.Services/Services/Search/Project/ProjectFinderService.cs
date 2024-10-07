using System.Runtime.CompilerServices;
using Dapper;
using LeokaEstetica.Platform.Base.Abstractions.Repositories.User;
using LeokaEstetica.Platform.CallCenter.Abstractions.Resume;
using LeokaEstetica.Platform.Models.Entities.User;
using LeokaEstetica.Platform.Models.Enums;
using LeokaEstetica.Platform.Notifications.Abstractions;
using LeokaEstetica.Platform.Notifications.Consts;
using LeokaEstetica.Platform.Services.Abstractions.Search.Project;
using Microsoft.Extensions.Logging;

[assembly: InternalsVisibleTo("LeokaEstetica.Platform.Tests")]

namespace LeokaEstetica.Platform.Services.Services.Search.Project;

/// <summary>
/// Класс реализует методы сервиса поиска в проектах.
/// </summary>
internal sealed class ProjectFinderService : IProjectFinderService
{
    private readonly ILogger<ProjectFinderService> _logger;
    private readonly IUserRepository _userRepository;
    private readonly IResumeModerationService _resumeModerationService;
    private readonly Lazy<IHubNotificationService> _hubNotificationService;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="logger">Логгер.</param>
    /// <param name="userRepository">Репозиторий пользователей.</param>
    /// <param name="resumeModerationService">Сервис модерации анкет.</param>
    /// <param name="resumeModerationService">Сервис уведомлений.</param>
    public ProjectFinderService(ILogger<ProjectFinderService> logger,
        IUserRepository userRepository,
        IResumeModerationService resumeModerationService,
        Lazy<IHubNotificationService> hubNotificationService)
    {
        _logger = logger;
        _userRepository = userRepository;
        _resumeModerationService = resumeModerationService;
        _hubNotificationService = hubNotificationService;
    }

    /// <inheritdoc />
    public async Task<UserEntity> SearchInviteProjectMembersAsync(string searchText, string token, string account)
    {
        try
        {
            // Находим Id текущего пользователя, который просматривает страницу проекта или вакансии.
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId == 0)
            {
                throw new InvalidOperationException($"Id пользователя с аккаунтом {account} не найден.");
            }
            
            var users = (await _userRepository.GetUserByEmailOrLoginAsync(searchText))?.AsList();
            
            // Если не нашли или даже если более 1, то все равно это ошибка.
            // Значит не заполнили полносттью почту пользователя в поиске и все равно не светим
            // пользователей по частичному совпадению. Нам нужно именно полное совпадение по почте
            // иначе говорим, что нет пользователя.
            if (users is null || users.Count is 0 or > 1)
            {
                var ex = new InvalidOperationException($"Пользователя с почтой {searchText} не найдено.");
                var userCode = await _userRepository.GetUserCodeByUserIdAsync(userId);

                await _hubNotificationService.Value.SendNotificationAsync("Внимание",
                    ex.Message,
                    NotificationLevelConsts.NOTIFICATION_LEVEL_WARNING, "SendNotificationWarningNotFoundUserByEmail",
                    userCode, UserConnectionModuleEnum.Main);

                return new UserEntity();
            }

            // Получаем анкеты на модерации.
            var resumesModeration = await _resumeModerationService.ResumesModerationAsync();
            
            // TODO: Делать все это в запросе выше, который переписан уже на Dapper.
            // Отбираем пользователей, которые на модерации и удалим их из выборки.
            var removedUsers = resumesModeration.Resumes
                .IntersectBy(users.Select(x => x.UserId), u => u.UserId)
                .AsList();
            
            if (removedUsers.Any())
            {
                // TODO: Делать все это в запросе выше, который переписан уже на Dapper.
                users.RemoveAll(u => removedUsers.Select(x => x.UserId).Contains(u.UserId));
            }

            return users.FirstOrDefault() ?? new UserEntity();
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<UserEntity?> SearchUserByEmailAsync(string searchText, string account)
    {
        try
        {
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId == 0)
            {
                throw new InvalidOperationException($"Id пользователя с аккаунтом {account} не найден.");
            }
            
            var users = (await _userRepository.GetUserByEmailOrLoginAsync(searchText))?.AsList();
            
            // Если не нашли или даже если более 1, то все равно это ошибка.
            // Значит не заполнили полносттью почту пользователя в поиске и все равно не светим
            // пользователей по частичному совпадению. Нам нужно именно полное совпадение по почте
            // иначе говорим, что нет пользователя.
            if (users is null || users.Count is 0 or > 1)
            {
                var ex = new InvalidOperationException($"Пользователя с почтой {searchText} не найдено.");
                var userCode = await _userRepository.GetUserCodeByUserIdAsync(userId);

                await _hubNotificationService.Value.SendNotificationAsync("Внимание",
                    ex.Message,
                    NotificationLevelConsts.NOTIFICATION_LEVEL_WARNING, "SendNotificationWarningNotFoundUserByEmail",
                    userCode, UserConnectionModuleEnum.ProjectManagement);

                throw ex;
            }

            return users.First();
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }
}