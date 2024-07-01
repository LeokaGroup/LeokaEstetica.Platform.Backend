using Dapper;
using LeokaEstetica.Platform.Base.Abstractions.Repositories.User;
using LeokaEstetica.Platform.Core.Exceptions;
using LeokaEstetica.Platform.Database.Abstractions.Project;
using LeokaEstetica.Platform.Database.Abstractions.ProjectManagment;
using LeokaEstetica.Platform.Models.Dto.Output.Notification;
using LeokaEstetica.Platform.Models.Dto.Output.ProjectManagement.Output;
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
    private readonly IProjectManagementSettingsRepository _projectManagementSettingsRepository;
    private readonly IProjectRepository _projectRepository;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="logger">Логгер.</param>
    /// <param name="userRepository">Репозиторий пользователей.</param>
    /// <param name="projectRepository">Репозиторий проектов.</param>
    public ProjectManagementSettingsService(ILogger<ProjectManagementSettingsService>? logger,
        IUserRepository userRepository,
        IProjectManagementSettingsRepository projectManagementSettingsRepository,
        IProjectRepository projectRepository)
    {
        _logger = logger;
        _userRepository = userRepository;
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

    /// <inheritdoc />
    public async Task UpdateProjectSprintsDurationSettingsAsync(long projectId, bool isSettingSelected, string sysName)
    {
        try
        {
            await _projectManagementSettingsRepository.UpdateProjectSprintsDurationSettingsAsync(projectId,
                isSettingSelected, sysName);
        }

        catch (Exception ex)
        {
            _logger?.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task UpdateProjectSprintsMoveNotCompletedTasksSettingsAsync(long projectId, bool isSettingSelected,
        string sysName)
    {
        try
        {
            await _projectManagementSettingsRepository.UpdateProjectSprintsMoveNotCompletedTasksSettingsAsync(
                projectId, isSettingSelected, sysName);
        }

        catch (Exception ex)
        {
            _logger?.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<IEnumerable<ProjectSettingUserOutput>> GetCompanyProjectUsersAsync(long projectId,
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

            var result = await _projectManagementSettingsRepository.GetCompanyProjectUsersAsync(projectId);

            return result;
        }
        
        catch (Exception ex)
        {
            _logger?.LogError(ex, ex.Message);
            throw;
        }
    }
    
    /// <inheritdoc />
    public async Task<IEnumerable<ProjectInviteOutput>> GetProjectInvitesAsync(long projectId)
    {
        try
        {
            var result = (await _projectManagementSettingsRepository.GetProjectInvitesAsync(projectId))?.AsList();

            if (result is null || result.Count == 0)
            {
                return Enumerable.Empty<ProjectInviteOutput>();
            }
            
            var projectOwnerId = await _projectRepository.GetProjectOwnerIdAsync(projectId);
            var findOwner = result.Find(x => x.UserId == projectOwnerId);

            if (findOwner is not null)
            {
                result.Remove(findOwner);
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