using LeokaEstetica.Platform.Base.Abstractions.Repositories.User;
using LeokaEstetica.Platform.Core.Exceptions;
using LeokaEstetica.Platform.Database.Abstractions.Config;
using LeokaEstetica.Platform.Services.Abstractions.Config;
using Microsoft.Extensions.Logging;

namespace LeokaEstetica.Platform.Services.Services.Config;

/// <inheritdoc />
internal sealed class ProjectSettingsConfigService : IProjectSettingsConfigService
{
    private readonly IProjectSettingsConfigRepository _projectSettingsConfigRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<ProjectSettingsConfigService> _logger;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="projectSettingsConfigRepository">Репозиторий настроек проектов.</param>
    /// <param name="userRepository">Репозиторий пользователей.</param>
    /// <param name="logger">Логгер.</param>
    public ProjectSettingsConfigService(IProjectSettingsConfigRepository projectSettingsConfigRepository,
        IUserRepository userRepository,
        ILogger<ProjectSettingsConfigService> logger)
    {
        _projectSettingsConfigRepository = projectSettingsConfigRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task CommitSpaceSettingsAsync(string strategy, int templateId, long projectId, string account)
    {
        try
        {
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                throw ex;
            }

            await _projectSettingsConfigRepository.CommitSpaceSettingsAsync(strategy, templateId, projectId, userId);
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }
}