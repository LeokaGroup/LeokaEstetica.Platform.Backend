using Dapper;
using LeokaEstetica.Platform.Base.Abstractions.Repositories.User;
using LeokaEstetica.Platform.Core.Exceptions;
using LeokaEstetica.Platform.Database.Abstractions.Project;
using LeokaEstetica.Platform.Database.Abstractions.ProjectManagment;
using LeokaEstetica.Platform.Integrations.Abstractions.Discord;
using LeokaEstetica.Platform.Models.Dto.ProjectManagement.Output;
using LeokaEstetica.Platform.Services.Abstractions.ProjectManagment;
using Microsoft.Extensions.Logging;

namespace LeokaEstetica.Platform.Services.Services.ProjectManagment;

/// <summary>
/// Класс реализует методы сервиса настроек проекта.
/// </summary>
internal sealed class ProjectManagementSettingsService : IProjectManagementSettingsService
{
    private readonly ILogger<ProjectManagementSettingsService>? _logger;
    private readonly IUserRepository _userRepository;
    private readonly IDiscordService _discordService;
    private readonly IProjectManagementSettingsRepository _projectManagementSettingsRepository;
    private readonly IProjectRepository _projectRepository;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="logger">Логгер.</param>
    /// <param name="userRepository">Репозиторий пользователей.</param>
    /// <param name="discordService">Сервис уведомлений дискорда.</param>
    /// <param name="discordService">Репозиторий настроек проекта.</param>
    /// <param name="projectRepository">Репозиторий проектов.</param>
    public ProjectManagementSettingsService(ILogger<ProjectManagementSettingsService>? logger,
        IUserRepository userRepository,
         IDiscordService discordService,
        IProjectManagementSettingsRepository projectManagementSettingsRepository,
        IProjectRepository projectRepository)
    {
        _logger = logger;
        _userRepository = userRepository;
        _discordService = discordService;
        _projectManagementSettingsRepository = projectManagementSettingsRepository;
        _projectRepository = projectRepository;
    }

    #region Публичные методы.

    /// <inheritdoc />
    public async Task<IEnumerable<SprintDurationSetting>> GetProjectSprintsDurationSettingsAsync(long projectId,
        string account)
    {
        try
        {
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                throw ex;
            }

            var result = (await _projectManagementSettingsRepository.GetProjectSprintsDurationSettingsAsync(projectId))
                .AsList();

            var isProjectOwner = await _projectRepository.CheckProjectOwnerAsync(projectId, userId);

            if (!isProjectOwner)
            {
                foreach (var s in result)
                {
                    s.Disabled = true;
                }
            }

            return result;
        }
        
        catch (Exception ex)
        {
            _logger?.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<IEnumerable<SprintMoveNotCompletedTaskSetting>>
        GetProjectSprintsMoveNotCompletedTasksSettingsAsync(long projectId, string account)
    {
        try
        {
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                throw ex;
            }

            var result = (await _projectManagementSettingsRepository
                .GetProjectSprintsMoveNotCompletedTasksSettingsAsync(projectId)).AsList();

            var isProjectOwner = await _projectRepository.CheckProjectOwnerAsync(projectId, userId);

            if (!isProjectOwner)
            {
                foreach (var s in result)
                {
                    s.Disabled = true;
                }
            }

            return result;
        }
        
        catch (Exception ex)
        {
            _logger?.LogError(ex, ex.Message);
            throw;
        }
    }

    #endregion

    #region Приватные методы.

    #endregion
}