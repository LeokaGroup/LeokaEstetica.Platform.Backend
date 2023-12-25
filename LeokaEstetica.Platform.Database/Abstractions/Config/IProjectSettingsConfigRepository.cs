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
    Task CommitSpaceSettingsAsync(string strategy, int templateId, long projectId, long userId, bool isProjectOwner,
        string redirectUrl);

    /// <summary>
    /// Метод получает Id проекта, который был ранее выбран пользователем для перехода к управлению проектом.
    /// Необходимо для построения ссылки в рабочее пространство проекта.
    /// </summary>
    /// <returns>Выходная модель.</returns>
    Task<ConfigSpaceSettingEntity> GetBuildProjectSpaceSettingsAsync(long userId);
}