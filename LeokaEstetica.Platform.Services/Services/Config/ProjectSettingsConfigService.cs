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
        long projectId, string account)
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
                GlobalConfigKeys.ConfigSpaceSetting.PROJECT_MANAGMENT_SPACE_URL);

            await _projectSettingsConfigRepository.CommitSpaceSettingsAsync(strategy, templateId, projectId, userId,
                isProjectOwner, redirectUrl);
                
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

            var result = new ConfigSpaceSettingOutput();

            var settings = await _projectSettingsConfigRepository.GetBuildProjectSpaceSettingsAsync(userId);
            
            if (settings is null)
            {
                throw new InvalidOperationException("Ошибка получения настроек проекта settings.");
            }

            result.IsCommitProjectSettings = settings.ProjectId > 0;

            if (!result.IsCommitProjectSettings)
            {
                throw new InvalidOperationException("Ошибка получения настроек проекта IsCommitProjectSettings.");
            }

            var projectId = settings.ProjectId;

            result.ProjectId = projectId;

            // Если ранее фиксировали шаблон и стратегию представления,
            // то можем сформировать строку в раб.пространство сразу.
            // Иначе фронт будем предлагать к выбору проект, шаблон и стратегию представления.
            if (result.IsCommitProjectSettings)
            {
                result.ProjectManagmentSpaceUrl = string.Concat(settings.ParamValue, $"?projectId={projectId}");
            }

            return result;
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }
}