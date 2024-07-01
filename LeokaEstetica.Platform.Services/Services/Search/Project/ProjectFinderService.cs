using System.Runtime.CompilerServices;
using LeokaEstetica.Platform.Base.Abstractions.Repositories.User;
using LeokaEstetica.Platform.CallCenter.Abstractions.Resume;
using LeokaEstetica.Platform.Models.Entities.User;
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
    private readonly IProjectNotificationsService _projectNotificationsService;
    private readonly IResumeModerationService _resumeModerationService;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="logger">Логгер.</param>
    /// <param name="userRepository">Репозиторий пользователей.</param>
    /// <param name="projectNotificationsService">Сервис уведомлений проектов.</param>
    /// <param name="resumeModerationService">Сервис модерации анкет.</param>
    public ProjectFinderService(ILogger<ProjectFinderService> logger,
        IUserRepository userRepository,
        IProjectNotificationsService projectNotificationsService,
        IResumeModerationService resumeModerationService)
    {
        _logger = logger;
        _userRepository = userRepository;
        _projectNotificationsService = projectNotificationsService;
        _resumeModerationService = resumeModerationService;
    }

    /// <summary>
    /// Метод ищет пользователей для приглашения в команду проекта.
    /// </summary>
    /// <param name="searchText">Поисковый запрос.</param>
    /// <param name="token">Токен пользователя.</param>
    /// <returns>Список пользователей, которых можно пригласить в команду проекта.</returns>
    public async Task<IEnumerable<UserEntity>> SearchInviteProjectMembersAsync(string searchText, string token)
    {
        try
        {
            var users = await _userRepository.GetUserByEmailOrLoginAsync(searchText);
            
            if (users is null || users.Count == 0)
            {
                return Enumerable.Empty<UserEntity>();
            }

            // Получаем анкеты на модерации.
            var resumesModeration = await _resumeModerationService.ResumesModerationAsync();
            
            // TODO: Делать все это в запросе выше, который переписан уже на Dapper.
            // Отбираем пользователей, которые на модерации и удалим их из выборки.
            var removedUsers = resumesModeration.Resumes
                .IntersectBy(users.Select(x => x.UserId), u => u.UserId)
                .ToList();
            
            if (removedUsers.Any())
            {
                // TODO: Делать все это в запросе выше, который переписан уже на Dapper.
                users.RemoveAll(u => removedUsers.Select(x => x.UserId).Contains(u.UserId));
            }

            return users;
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }
}