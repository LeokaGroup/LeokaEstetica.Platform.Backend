using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Core.Data;
using LeokaEstetica.Platform.Database.Abstractions.Config;
using LeokaEstetica.Platform.Models.Entities.Configs;

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
    public async Task CommitSpaceSettingsAsync(string strategy, int templateId, long projectId, long userId)
    {
        await _pgContext.AddRangeAsync(new List<ConfigSpaceSettingEntity>
        {
            new ()
            {
                ParamKey = GlobalConfigKeys.ConfigSpaceSetting.PROJECT_MANAGEMENT_STRATEGY,
                ParamValue = strategy,
                ParamType = "String",
                UserId = userId,
                ProjectId = projectId,
                ParamDescription = "Выбранная пользователем стратегия представления.",
                ParamTag = "Project management settings"
            },
            new ()
            {
                ParamKey = GlobalConfigKeys.ConfigSpaceSetting.PROJECT_MANAGMENT_TEMPLATE_ID,
                ParamValue = templateId.ToString(),
                ParamType = "Int64",
                UserId = userId,
                ProjectId = projectId,
                ParamDescription = "Выбранный пользователем шаблон.",
                ParamTag = "Project management settings"
            }
        });
        await _pgContext.SaveChangesAsync();
    }
}