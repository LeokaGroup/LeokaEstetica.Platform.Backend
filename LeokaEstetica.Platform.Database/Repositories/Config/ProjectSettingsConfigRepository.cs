using Dapper;
using LeokaEstetica.Platform.Base.Abstractions.Connection;
using LeokaEstetica.Platform.Base.Abstractions.Repositories.Base;
using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Core.Data;
using LeokaEstetica.Platform.Database.Abstractions.Config;
using LeokaEstetica.Platform.Models.Dto.Output.ProjectManagement.Output;
using LeokaEstetica.Platform.Models.Entities.Configs;
using Microsoft.EntityFrameworkCore;

namespace LeokaEstetica.Platform.Database.Repositories.Config;

/// <inheritdoc />
internal sealed class ProjectSettingsConfigRepository : BaseRepository, IProjectSettingsConfigRepository
{
    private readonly PgContext _pgContext;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="pgContext">Датаконтекст.</param>
    public ProjectSettingsConfigRepository(PgContext pgContext, IConnectionProvider connectionProvider)
        : base(connectionProvider)
    {
        _pgContext = pgContext;
    }

    /// <inheritdoc />
    public async Task CommitSpaceSettingsAsync(string strategy, int templateId, long projectId, long userId,
        bool isProjectOwner, string redirectUrl, string projectManagementName, string projectManagementNamePrefix)
    {
        var settings = new List<ConfigSpaceSettingEntity>();

        // Для владельца проекта запоминаем еще и выбранный шаблон проекта.
        // Другие участники проекта будут следовать этому шаблону, так как конфигурацию имеет право задавать
        // лишь владелец проекта и пользователи, у которых есть нужная роль.
        if (isProjectOwner)
        {
            settings.Add(new ConfigSpaceSettingEntity
            {
                ParamKey = GlobalConfigKeys.ConfigSpaceSetting.PROJECT_MANAGEMENT_TEMPLATE_ID,
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
            new()
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
            new()
            {
                ParamKey = GlobalConfigKeys.ConfigSpaceSetting.PROJECT_MANAGEMENT_SPACE_URL,
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

        settings.Add(new ConfigSpaceSettingEntity
        {
            ParamKey = GlobalConfigKeys.ConfigSpaceSetting.PROJECT_MANAGEMENT_PROJECT_NAME,
            ParamValue = projectManagementName,
            ParamType = "String",
            UserId = userId,
            ProjectId = projectId,
            ParamDescription = "Название проекта в модуле УП.",
            ParamTag = "Project management settings",
            LastUserDate = DateTime.UtcNow
        });

        settings.Add(new ConfigSpaceSettingEntity
        {
            ParamKey = GlobalConfigKeys.ConfigSpaceSetting.PROJECT_MANAGEMENT_PROJECT_NAME_PREFIX,
            ParamValue = projectManagementNamePrefix,
            ParamType = "String",
            UserId = userId,
            ProjectId = projectId,
            ParamDescription = "Префикс названия проекта в модуле УП.",
            ParamTag = "Project management settings",
            LastUserDate = DateTime.UtcNow
        });

        await _pgContext.ConfigSpaceSettings.AddRangeAsync(settings);
        await _pgContext.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task<IEnumerable<ConfigSpaceSettingEntity>> GetBuildProjectSpaceSettingsAsync(long userId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var parameters = new DynamicParameters();
        parameters.Add("@userId", userId);

        var query = "SELECT \"ProjectId\", \"ParamValue\", \"ParamKey\" " +
                    "FROM \"Configs\".\"ProjectManagmentProjectSettings\" " +
                    "WHERE \"UserId\" = @userId";

        // Получаем настройки пользователя.
        var result = (await connection.QueryAsync<ConfigSpaceSettingEntity>(query, parameters))?.AsList();

        // Дополнительно роверим, есть ли пользователь в участниках проектов не своих.
        var checkMemberProjects = "SELECT pt.\"ProjectId\", ptm.\"UserId\" " +
                                  "FROM \"Teams\".\"ProjectsTeamsMembers\" AS ptm " +
                                  "INNER JOIN \"Teams\".\"ProjectsTeams\" AS pt " +
                                  "ON ptm.\"TeamId\" = pt.\"TeamId\" " +
                                  "WHERE ptm.\"UserId\" = @userId";

        var projectMembers = (await connection.QueryAsync<ProjectTeamMemberOutput>(
                checkMemberProjects, parameters))
            ?.AsList();

        // Промежуточный список для наполнения настроек в результате.
        projectMembers ??= new List<ProjectTeamMemberOutput>();

        // Наполняем настройками, если текущий пользователь есть в участниках других проектов.
        var otherSettingsParameters = new DynamicParameters();
        otherSettingsParameters.Add("@userId", userId);
        otherSettingsParameters.Add("@projectMemberIds", projectMembers.Select(x => x.UserId).AsList());

        // Находим настройки проектов, в которых в участниках текущий пользователь.
        var otherSettings = "SELECT \"ProjectId\", \"ParamValue\", \"ParamKey\", \"UserId\", \"ParamType\"," +
                            " \"ParamTag\", \"ParamDescription\" " +
                            "FROM \"Configs\".\"ProjectManagmentProjectSettings\" " +
                            "WHERE \"UserId\" = ANY(@projectMemberIds)";

        var otherSettingsResult = (await connection.QueryAsync<ConfigSpaceSettingEntity>(
            otherSettings, otherSettingsParameters))?.AsList();
        
        otherSettingsResult ??= new List<ConfigSpaceSettingEntity>();
        
        // Добавляем в настройки проектов.
        var insertSettings = "INSERT INTO \"Configs\".\"ProjectManagmentProjectSettings\" (" +
                             "\"ProjectId\", \"ParamValue\", \"ParamKey\", \"UserId\", \"ParamType\"," +
                             " \"ParamTag\", \"ParamDescription\") " +
                             "VALUES (@projectId, @value, @key, @userId, @type, @tag, @description)";

        foreach (var p in otherSettingsResult)
        {
            var insertSettingsParameters = new DynamicParameters();
            insertSettingsParameters.Add("@projectId", p.ProjectId);
            insertSettingsParameters.Add("@key", p.ParamKey);
            insertSettingsParameters.Add("@value", p.ParamValue);
            insertSettingsParameters.Add("@userId", userId);
            insertSettingsParameters.Add("@type", p.ParamType);
            insertSettingsParameters.Add("@tag", p.ParamTag);
            insertSettingsParameters.Add("@description", p.ParamDescription);
            
            // Проверяем, заведены ли настройки.
            var checkSettingsParameters = new DynamicParameters();
            checkSettingsParameters.Add("@userId", userId);
            checkSettingsParameters.Add("@projectId", p.ProjectId);

            var checkSettings = "SELECT EXISTS (SELECT \"ProjectId\", \"UserId\" " +
                                "FROM \"Configs\".\"ProjectManagmentProjectSettings\" " +
                                "WHERE \"ProjectId\" = @projectId " +
                                "AND \"UserId\" = @userId)";

            var isExists = await connection.ExecuteScalarAsync<bool>(checkSettings, checkSettingsParameters);

            if (!isExists)
            {
                await connection.ExecuteAsync(insertSettings, insertSettingsParameters);   
            }
        }

        result ??= new List<ConfigSpaceSettingEntity>();

        // Наполняем результат настройками.
        foreach (var s in otherSettingsResult)
        {
            result.Add(new ConfigSpaceSettingEntity
            {
                UserId = s.UserId,
                ProjectId = s.ProjectId,
                ParamKey = s.ParamKey,
                ParamValue = s.ParamValue,
                ParamTag = s.ParamTag,
                ParamDescription = s.ParamDescription,
                ParamType = s.ParamType
            });
        }

        return result;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<ConfigSpaceSettingEntity>?> GetProjectSpaceSettingsByProjectIdAsync(long projectId,
        long userId)
    {
        var result = new List<ConfigSpaceSettingEntity>();

        // Получаем шаблон проекта, который был задан владельцем проекта.
        var projectTemplate = await _pgContext.ConfigSpaceSettings
            .Where(s => s.ProjectId == projectId
                        && s.ParamKey.Equals(GlobalConfigKeys.ConfigSpaceSetting.PROJECT_MANAGEMENT_TEMPLATE_ID))
            .Select(s => new ConfigSpaceSettingEntity
            {
                ParamKey = s.ParamKey,
                ParamValue = s.ParamValue
            })
            .FirstOrDefaultAsync();

        if (projectTemplate is not null)
        {
            result.Add(projectTemplate);
        }

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
            
        if (userStrategy is not null)
        {
            result.Add(userStrategy);
        }

        // Получаем роут представления с задачами.
        var url = await _pgContext.ConfigSpaceSettings
            .Where(s => s.ProjectId == projectId
                        && s.ParamKey.Equals(GlobalConfigKeys.ConfigSpaceSetting.PROJECT_MANAGEMENT_SPACE_URL))
            .Select(s => new ConfigSpaceSettingEntity
            {
                ParamKey = s.ParamKey,
                ParamValue = s.ParamValue
            })
            .FirstOrDefaultAsync();
            
        if (url is not null)
        {
            result.Add(url);
        }

        return result;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<ConfigSpaceSettingEntity>> GetProjectSpaceSettingsByProjectIdAsync(long projectId)
    {
        var result = new List<ConfigSpaceSettingEntity>();

        // Получаем шаблон проекта, который был задан владельцем проекта.
        var projectTemplate = await _pgContext.ConfigSpaceSettings
            .Where(s => s.ProjectId == projectId
                        && s.ParamKey.Equals(GlobalConfigKeys.ConfigSpaceSetting.PROJECT_MANAGEMENT_TEMPLATE_ID))
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
                        && s.ParamKey.Equals(GlobalConfigKeys.ConfigSpaceSetting.PROJECT_MANAGEMENT_STRATEGY))
            .Select(s => new ConfigSpaceSettingEntity
            {
                ParamKey = s.ParamKey,
                ParamValue = s.ParamValue
            })
            .FirstOrDefaultAsync();

        result.Add(userStrategy);

        // Получаем роута представления с задачами.
        var url = await _pgContext.ConfigSpaceSettings
            .Where(s => s.ProjectId == projectId
                        && s.ParamKey.Equals(GlobalConfigKeys.ConfigSpaceSetting.PROJECT_MANAGEMENT_SPACE_URL))
            .Select(s => new ConfigSpaceSettingEntity
            {
                ParamKey = s.ParamKey,
                ParamValue = s.ParamValue
            })
            .FirstOrDefaultAsync();

        result.Add(url);

        return result;
    }
}