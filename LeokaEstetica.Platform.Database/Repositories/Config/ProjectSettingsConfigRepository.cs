using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Core.Data;
using LeokaEstetica.Platform.Database.Abstractions.Config;
using LeokaEstetica.Platform.Models.Entities.Configs;
using Microsoft.EntityFrameworkCore;

namespace LeokaEstetica.Platform.Database.Repositories.Config;

/// <inheritdoc />
internal sealed class ProjectSettingsConfigRepository : IProjectSettingsConfigRepository
{
    private readonly PgContext _pgContext;
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="pgContext">Датаконтекст.</param>
    public ProjectSettingsConfigRepository(PgContext pgContext)
    {
        _pgContext = pgContext;
    }

    /// <inheritdoc />
    public async Task CommitSpaceSettingsAsync(string strategy, int templateId, long projectId, long userId,
        bool isProjectOwner, string redirectUrl)
    {
        var settings = new List<ConfigSpaceSettingEntity>();

        // Для владельца проекта запоминаем еще и выбранный шаблон проекта.
        // Другие участники проекта будут следовать этому шаблону, так как конфигурацию имеет право задавать
        // лишь владелец проекта и пользователи, у которых есть нужная роль.
        if (isProjectOwner)
        {
            settings.Add(new ConfigSpaceSettingEntity
            {
                ParamKey = GlobalConfigKeys.ConfigSpaceSetting.PROJECT_MANAGMENT_TEMPLATE_ID,
                ParamValue = templateId.ToString(),
                ParamType = "Int64",
                UserId = userId,
                ProjectId = projectId,
                ParamDescription = "Выбранный пользователем шаблон.",
                ParamTag = "Project management settings",
                LastUserDate = DateTime.UtcNow
            });
        }

        settings.AddRange(new List<ConfigSpaceSettingEntity>
        {
            new ()
            {
                ParamKey = GlobalConfigKeys.ConfigSpaceSetting.PROJECT_MANAGEMENT_STRATEGY,
                ParamValue = strategy,
                ParamType = "String",
                UserId = userId,
                ProjectId = projectId,
                ParamDescription = "Выбранная пользователем стратегия представления.",
                ParamTag = "Project management settings",
                LastUserDate = DateTime.UtcNow
            },
            new ()
            {
                ParamKey = GlobalConfigKeys.ConfigSpaceSetting.PROJECT_MANAGMENT_SPACE_URL,
                ParamValue = redirectUrl,
                ParamType = "String",
                UserId = userId,
                ProjectId = projectId,
                ParamDescription = "Url редиректа к управлению проектом." +
                                   " Когда ранее был зафиксирован шаблон, стратегия представления и выбран проект.",
                ParamTag = "Project management settings",
                LastUserDate = DateTime.UtcNow
            }
        });

        await _pgContext.ConfigSpaceSettings.AddRangeAsync(settings);
        await _pgContext.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task<ConfigSpaceSettingEntity> GetBuildProjectSpaceSettingsAsync(long userId)
    {
        var result = await _pgContext.ConfigSpaceSettings
            .Where(c => c.UserId == userId
                        && c.ParamKey.Equals(GlobalConfigKeys.ConfigSpaceSetting.PROJECT_MANAGMENT_SPACE_URL))
            .OrderByDescending(o => o.LastUserDate)
            .Select(c => new ConfigSpaceSettingEntity
            {
                ProjectId = c.ProjectId,
                ParamValue = c.ParamValue
            })
            .FirstOrDefaultAsync();

        return result;
    }

    /// <summary>
    /// Метод получает настройки по Id проекта.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Данные конфигурации.</returns>
    public async Task<IEnumerable<ConfigSpaceSettingEntity>> GetProjectSpaceSettingsByProjectIdAsync(long projectId,
        long userId)
    {
        var result = new List<ConfigSpaceSettingEntity>();

        // Получаем шаблон проекта, который был задан владельцем проекта.
        var projectTemplate = await _pgContext.ConfigSpaceSettings
            .Where(s => s.ProjectId == projectId
                        && s.ParamKey.Equals(GlobalConfigKeys.ConfigSpaceSetting.PROJECT_MANAGMENT_TEMPLATE_ID))
            .Select(s => new ConfigSpaceSettingEntity
            {
                ParamKey = s.ParamKey,
                ParamValue = s.ParamValue
            })
            .FirstOrDefaultAsync();

        result.Add(projectTemplate);
        
        // Получаем стратегию пользователя в этом проекте.
        var userStrategy = await _pgContext.ConfigSpaceSettings
            .Where(s => s.ProjectId == projectId
                        && s.ParamKey.Equals(GlobalConfigKeys.ConfigSpaceSetting.PROJECT_MANAGEMENT_STRATEGY)
                        && s.UserId == userId)
            .Select(s => new ConfigSpaceSettingEntity
            {
                ParamKey = s.ParamKey,
                ParamValue = s.ParamValue
            })
            .FirstOrDefaultAsync();
        
        result.Add(userStrategy);

        return result;
    }
}