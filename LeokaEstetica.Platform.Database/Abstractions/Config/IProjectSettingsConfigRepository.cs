using LeokaEstetica.Platform.Models.Entities.Configs;

namespace LeokaEstetica.Platform.Database.Abstractions.Config;

/// <summary>
/// Абстракция репозитория настроек проектов.
/// </summary>
public interface IProjectSettingsConfigRepository
{
    /// <summary>
    /// Метод фиксирует выбранные пользователем настройки рабочего пространства проекта.
    /// </summary>
    /// <param name="strategy">Стратегия представления.</param>
    /// <param name="templateId">Id шаблона.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="isProjectOwner">Признак владельца проекта.</param>
    /// <param name="redirectUrl">Url редиректа в рабочее пространство проекта.</param>
    /// <param name="projectManagementName">Название в управлении проектом.</param>
    /// <param name="projectManagementNamePrefix">Префикс названия в управлении проектом.</param>
    Task CommitSpaceSettingsAsync(string strategy, int templateId, long projectId, long userId, bool isProjectOwner,
        string redirectUrl, string projectManagementName, string projectManagementNamePrefix);

    /// <summary>
    /// Метод получает настройки и Id проекта.
    /// </summary>
    /// <returns>Данные конфигурации.</returns>
    Task<(IEnumerable<ConfigSpaceSettingEntity> Settings, long ProjectId)>
        GetBuildProjectSpaceSettingsAsync(long userId);

    /// <summary>
    /// TODO: Отрефаить этот метод с/без UserId.
    /// TODO: Чтобы можно было удалить метод GetProjectSpaceSettingsByProjectIdAsync (который без параметра UserId)
    /// TODO: и переиспользовать его логику как с так и без UserId.
    /// Метод получает настройки по Id проекта.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Данные конфигурации.</returns>
    Task<IEnumerable<ConfigSpaceSettingEntity>> GetProjectSpaceSettingsByProjectIdAsync(long projectId, long userId);
    
    /// <summary>
    /// TODO: Отрефаить этот метод, смотреть тудушку к методу GetProjectSpaceSettingsByProjectIdAsync с UserId.
    /// Метод получает настройки по Id проекта.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Данные конфигурации.</returns>
    Task<IEnumerable<ConfigSpaceSettingEntity>> GetProjectSpaceSettingsByProjectIdAsync(long projectId);
}