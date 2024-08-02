using Dapper;
using LeokaEstetica.Platform.Base.Abstractions.Repositories.User;
using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Core.Exceptions;
using LeokaEstetica.Platform.Database.Abstractions.Config;
using LeokaEstetica.Platform.Database.Abstractions.Project;
using LeokaEstetica.Platform.Models.Dto.Output.Configs;
using LeokaEstetica.Platform.Models.Entities.Configs;
using LeokaEstetica.Platform.Services.Abstractions.Config;
using Microsoft.Extensions.Logging;

namespace LeokaEstetica.Platform.Services.Services.Config;

/// <inheritdoc />
internal sealed class ProjectSettingsConfigService : IProjectSettingsConfigService
{
    private readonly IProjectSettingsConfigRepository _projectSettingsConfigRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<ProjectSettingsConfigService> _logger;
    private readonly IProjectRepository _projectRepository;
    private readonly IGlobalConfigRepository _globalConfigRepository;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="projectSettingsConfigRepository">Репозиторий настроек проектов.</param>
    /// <param name="userRepository">Репозиторий пользователей.</param>
    /// <param name="logger">Логгер.</param>
    /// <param name="projectRepository">Репозиторий проектов.</param>
    /// <param name="globalConfigRepository">Репозиторий глобал конфига.</param>
    public ProjectSettingsConfigService(IProjectSettingsConfigRepository projectSettingsConfigRepository,
        IUserRepository userRepository,
        ILogger<ProjectSettingsConfigService> logger,
        IProjectRepository projectRepository,
        IGlobalConfigRepository globalConfigRepository)
    {
        _projectSettingsConfigRepository = projectSettingsConfigRepository;
        _userRepository = userRepository;
        _logger = logger;
        _projectRepository = projectRepository;
        _globalConfigRepository = globalConfigRepository;
    }

    #region Публичные методы.

    /// <inheritdoc />
    public async Task<ConfigSpaceSettingOutput> CommitSpaceSettingsAsync(string strategy, int templateId,
        long projectId, string account, string projectManagementName, string projectManagementNamePrefix)
    {
        try
        {
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                throw ex;
            }

            // Проверяем, является ли текущий пользователь владельцем проекта.
            var isProjectOwner = await _projectRepository.CheckProjectOwnerAsync(projectId, userId);
            var redirectUrl = await _globalConfigRepository.GetValueByKeyAsync<string>(
                GlobalConfigKeys.ConfigSpaceSetting.PROJECT_MANAGEMENT_SPACE_URL);

            await _projectSettingsConfigRepository.CommitSpaceSettingsAsync(strategy, templateId, projectId, userId,
                isProjectOwner, redirectUrl, projectManagementName, projectManagementNamePrefix);

            await _projectRepository.SetProjectManagementNameAsync(projectId, projectManagementName);
                
            var result = new ConfigSpaceSettingOutput
            {
                ProjectId = projectId,
                ProjectManagmentSpaceUrl = string.Concat(redirectUrl, $"?projectId={projectId}"),
                IsCommitProjectSettings = true
            };

            return result;
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<ConfigSpaceSettingOutput> GetBuildProjectSpaceSettingsAsync(string account, long? projectId)
    {
        try
        {
            var result = new ConfigSpaceSettingOutput();
            
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (!projectId.HasValue)
            {
                // Автоматически редиректим в общее пространство.
                result.IsDefaultSpaceUrl = true;
                result.IsCommitProjectSettings = false;
                
                return result;
            }

            var projectOwnerId = await _projectRepository.GetProjectOwnerIdAsync(projectId.Value);

            // Для получения настроек проекта Id пользователя = Id владельца проекта.
            var settings = (await _projectSettingsConfigRepository.GetBuildProjectSpaceSettingsAsync(projectOwnerId))
                ?.Where(x => x?.ParamKey is not null)
                ?.Distinct()
                ?.AsList();

            // Если нет настроек, то это будет переход в общее пространство.
            if (settings is null || settings.Count == 0)
            {
                result.IsCommitProjectSettings = false;
                
                // Редиректим в общее пространство.
                result.IsDefaultSpaceUrl = true;
                result.ProjectId = projectId.Value;
                
                return result;
            }

            var findSettings = await FindProjectSettingsAsync(settings, projectId.Value, projectOwnerId);

            var isOwner = await _projectRepository.CheckProjectOwnerAsync(projectId.Value, userId);
            
            var redirectUrl = await _globalConfigRepository.GetValueByKeyAsync<string>(
                GlobalConfigKeys.ConfigSpaceSetting.PROJECT_MANAGEMENT_SPACE_URL);

            // Если все настройки были заполнены пользователем, то разрешаем генерацию ссылки в раб.пространство.
            if (findSettings.TemplateId > 0
                && !string.IsNullOrEmpty(findSettings.Strategy)
                && !string.IsNullOrEmpty(findSettings.ProjectManagementName)
                && !string.IsNullOrEmpty(findSettings.ProjectManagementNamePrefix))
            {
                // Настройки проекта были зафиксированы пользователем, делаем переход в раб.пространство этого проекта.
                result.ProjectId = projectId.Value;
                result.IsDefaultSpaceUrl = false;
                result.ProjectManagmentSpaceUrl = string.Concat(redirectUrl, $"?projectId={projectId.Value}");
                result.IsCommitProjectSettings = true;
               

                return result;
            }
            
            // Для не владельца просто фиксируем ему стратегию представления,
            // остальные настройки оставляем как задано владельцем проекта.
            if (!isOwner)
            {
                var notOwnerSettings = await FindProjectSettingsAsync(settings, projectId.Value, userId);

                // Фиксируемм настройки проекта новому пользователю команды проекта.
                // Отталкиваемся от настроек проекта, которые задал владелец проекта.
                // По дефолту проставляем стратегию канбана.
                if (string.IsNullOrWhiteSpace(notOwnerSettings.Strategy))
                {
                    await _projectSettingsConfigRepository.CommitSpaceSettingsAsync("kn", findSettings.TemplateId,
                        projectId.Value, userId, false, redirectUrl, findSettings.ProjectManagementName!,
                        findSettings.ProjectManagementNamePrefix!);
                        
                    result.IsCommitProjectSettings = true;
                }

                else
                {
                    result.IsCommitProjectSettings = false;
                }

                result.ProjectId = projectId.Value;
                result.IsDefaultSpaceUrl = false;
                result.ProjectManagmentSpaceUrl = string.Concat(redirectUrl, $"?projectId={projectId.Value}");
                                    
                return result;
            }
            
            // Если не были зафиксированы настройки и владелец, то требуем заполнить настройки проекта.
            if (isOwner)
            {
                result.IsCommitProjectSettings = false;
                result.ProjectId = projectId.Value;
                result.IsDefaultSpaceUrl = false;
                result.ProjectManagmentSpaceUrl = string.Concat(redirectUrl, $"?projectId={projectId.Value}");

                return result;
            }

            throw new InvalidOperationException(
                "Не сработал ни один из кейсов проверки фиксации настроек проекта. " +
                $"ProjectId: {projectId}. " +
                $"UserId: {userId}.");
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    #endregion

    #region Приватные методы.

    /// <summary>
    /// Метод находит в настройках проекта варианты для конкретного пользователя.
    /// </summary>
    /// <param name="settings">Настройки проекта.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Найденные настройки.</returns>
    private async Task<(int TemplateId, string? Strategy, string? ProjectManagementName,
        string? ProjectManagementNamePrefix)> FindProjectSettingsAsync(List<ConfigSpaceSettingEntity> settings,
        long projectId, long userId)
    {
        var templateId = Convert.ToInt32(settings.Find(x =>
                x.ParamKey.Equals(GlobalConfigKeys.ConfigSpaceSetting.PROJECT_MANAGEMENT_TEMPLATE_ID)
                && x.ProjectId == projectId)
            ?.ParamValue);

        var strategy = settings.Find(x =>
                x.ParamKey.Equals(GlobalConfigKeys.ConfigSpaceSetting.PROJECT_MANAGEMENT_STRATEGY)
                && x.ProjectId == projectId
                && x.UserId == userId)
            ?.ParamValue;

        var projectManagementName = settings.Find(x =>
                x.ParamKey.Equals(GlobalConfigKeys.ConfigSpaceSetting.PROJECT_MANAGEMENT_PROJECT_NAME)
                && x.ProjectId == projectId)
            ?.ParamValue;

        var projectManagementNamePrefix = settings.Find(x =>
                x.ParamKey.Equals(GlobalConfigKeys.ConfigSpaceSetting.PROJECT_MANAGEMENT_PROJECT_NAME_PREFIX)
                && x.ProjectId == projectId)
            ?.ParamValue;
        
        return await Task.FromResult((templateId, strategy, projectManagementName, projectManagementNamePrefix));
    }

    #endregion
}