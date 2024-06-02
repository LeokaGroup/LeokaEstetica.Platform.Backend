using Dapper;
using LeokaEstetica.Platform.Base.Abstractions.Repositories.User;
using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Core.Exceptions;
using LeokaEstetica.Platform.Database.Abstractions.Config;
using LeokaEstetica.Platform.Database.Abstractions.Project;
using LeokaEstetica.Platform.Models.Dto.Output.Configs;
using LeokaEstetica.Platform.Services.Abstractions.Config;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

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
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                throw ex;
            }

            var settings = (await _projectSettingsConfigRepository.GetBuildProjectSpaceSettingsAsync(userId))
                ?.AsList();
            var result = new ConfigSpaceSettingOutput();
            
            // Если нет настроек, то это будет переход в общее пространство.
            if (!projectId.HasValue || settings is null || settings.Count == 0)
            {
                result.IsCommitProjectSettings = false;
                
                // Редиректим в общее пространство.
                result.IsDefaultSpaceUrl = true;
                
                return result;
            }

            var projectIds = (settings.Count > 0
                    ? settings.Select(x => x.ProjectId)
                    : new List<long>())
                .Distinct()
                .AsList();

            // Если нашли невалидный Id проекта.
            if (!projectIds.All(x => x > 0))
            {
                throw new InvalidOperationException("Найдены невалидные Id проекта при получении настроек проекта. " +
                                                    $"ProjectIds: {JsonConvert.SerializeObject(projectIds)}");
            }
            
            // Если все настройки были заполнены пользователем, то разрешаем генерацию ссылки в раб.пространство.
            if (projectIds.Count is > 0 and 1 && projectIds.First() > 0)
            {
                var templateId = Convert.ToInt32(settings.Find(x =>
                    x.ParamKey.Equals(GlobalConfigKeys.ConfigSpaceSetting.PROJECT_MANAGEMENT_TEMPLATE_ID))?.ParamValue);

                var strategy = settings.Find(x =>
                    x.ParamKey.Equals(GlobalConfigKeys.ConfigSpaceSetting.PROJECT_MANAGEMENT_STRATEGY))?.ParamValue;
                
                var projectManagementName = settings.Find(x =>
                    x.ParamKey.Equals(GlobalConfigKeys.ConfigSpaceSetting.PROJECT_MANAGEMENT_PROJECT_NAME))?.ParamValue;

                var projectManagementNamePrefix = settings.Find(x =>
                        x.ParamKey.Equals(GlobalConfigKeys.ConfigSpaceSetting.PROJECT_MANAGEMENT_PROJECT_NAME_PREFIX))
                    ?.ParamValue;

                if (templateId > 0
                    && !string.IsNullOrEmpty(strategy)
                    && !string.IsNullOrEmpty(projectManagementName)
                    && !string.IsNullOrEmpty(projectManagementNamePrefix))
                {
                    // Настройки проекта были зафиксированы пользователем, делаем переход в раб.пространство этого проекта.
                    result.IsCommitProjectSettings = true;
                }
            }

            // Не все настройки были зафиксированы, требуем от пользователя заполнить их все.
            // Заполнение настроек последнего проекта, который был выбран пользователем.
            if (projectIds.Count is > 0 and 1 
                && projectIds.First() > 0
                && !result.IsCommitProjectSettings)
            {
                var spaceUrl = settings.Find(x =>
                        x.ParamKey.Equals(GlobalConfigKeys.ConfigSpaceSetting.PROJECT_MANAGEMENT_SPACE_URL)
                        && x.ProjectId == projectIds.First())
                    ?.ParamValue;
                
                result.ProjectManagmentSpaceUrl = spaceUrl;
                result.ProjectId = projectIds.First();
                
                return result;
            }

            // TODO: Подумать, можем ли запоминать как то, какой последний проект выбирал пользователь?
            // TODO: Пока редиректим в общее пространство просто.
            if (projectIds.Count is > 0 and > 1)
            {
                // Не сработал ни один кейс при получении настроек проекта.
                // Автоматически редиректим в общее пространство.
                result.IsDefaultSpaceUrl = true;

                return result;
            }

            // Настройки были зафиксированы, переходим к проекту.
            if (result.IsCommitProjectSettings)
            {
                var spaceUrl = settings.Find(x =>
                        x.ParamKey.Equals(GlobalConfigKeys.ConfigSpaceSetting.PROJECT_MANAGEMENT_SPACE_URL)
                        && x.ProjectId == projectIds.First())
                    ?.ParamValue;
                
                result.ProjectManagmentSpaceUrl = string.Concat(spaceUrl, $"?projectId={projectIds.First()}");
                result.ProjectId = projectIds.First();
                
                return result;
            }

            // Не сработал ни один кейс при получении настроек проекта.
            // Автоматически редиректим в общее пространство.
            result.IsDefaultSpaceUrl = true;

            return result;
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }
}