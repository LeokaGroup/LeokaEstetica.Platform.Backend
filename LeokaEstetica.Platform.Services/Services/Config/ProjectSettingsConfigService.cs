using Dapper;
using LeokaEstetica.Platform.Base.Abstractions.Repositories.User;
using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Core.Exceptions;
using LeokaEstetica.Platform.Database.Abstractions.Config;
using LeokaEstetica.Platform.Database.Abstractions.Project;
using LeokaEstetica.Platform.Models.Dto.Output.Configs;
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
    public async Task<ConfigSpaceSettingOutput> GetBuildProjectSpaceSettingsAsync(string account)
    {
        try
        {
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                throw ex;
            }

            var settingsItems = await _projectSettingsConfigRepository.GetBuildProjectSpaceSettingsAsync(userId);
            var settings = settingsItems.Settings.AsList();
            
            if (settings is null)
            {
                throw new InvalidOperationException("Ошибка получения настроек проекта settings.");
            }
            
            var templateId = Convert.ToInt32(settings.Find(x =>
                x.ParamKey.Equals(GlobalConfigKeys.ConfigSpaceSetting.PROJECT_MANAGEMENT_TEMPLATE_ID))?.ParamValue);

            var strategy = settings.Find(x =>
                x.ParamKey.Equals(GlobalConfigKeys.ConfigSpaceSetting.PROJECT_MANAGEMENT_STRATEGY))?.ParamValue;
                
            var projectManagementName = settings.Find(x =>
                x.ParamKey.Equals(GlobalConfigKeys.ConfigSpaceSetting.PROJECT_MANAGEMENT_PROJECT_NAME))?.ParamValue;

            var projectManagementNamePrefix = settings.Find(x =>
                    x.ParamKey.Equals(GlobalConfigKeys.ConfigSpaceSetting.PROJECT_MANAGEMENT_PROJECT_NAME_PREFIX))
                ?.ParamValue;

            var projectId = settingsItems.ProjectId;
            var result = new ConfigSpaceSettingOutput();

            // Если все настройки были заполнены пользователем, то разрешаем генерацию ссылки в раб.пространство.
            if (projectId > 0
                && templateId > 0
                && !string.IsNullOrEmpty(strategy)
                && !string.IsNullOrEmpty(projectManagementName)
                && !string.IsNullOrEmpty(projectManagementNamePrefix))
            {
                result.IsCommitProjectSettings = true;
            }

            // Не все настройки были зафиксированы, требуем от пользователя заполнить их все.
            if (!result.IsCommitProjectSettings)
            {
                result.IsCommitProjectSettings = false;
                
                return result;
            }

            // Если ранее фиксировали настройки проекта, то можем сформировать ссылку в раб.пространство.
            // Иначе фронт будем предлагать недостающие настройки.
            var spaceUrl = settings.Find(x =>
                x.ParamKey.Equals(GlobalConfigKeys.ConfigSpaceSetting.PROJECT_MANAGEMENT_SPACE_URL))?.ParamValue;
            result.ProjectManagmentSpaceUrl = string.Concat(spaceUrl, $"?projectId={projectId}");

            return result;
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }
}