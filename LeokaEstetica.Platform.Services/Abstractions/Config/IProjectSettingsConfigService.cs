using LeokaEstetica.Platform.Models.Dto.Output.Configs;

namespace LeokaEstetica.Platform.Services.Abstractions.Config;

/// <summary>
/// Абстракция сервиса настроек проектов.
/// </summary>
public interface IProjectSettingsConfigService
{
    /// <summary>
    /// Метод фиксирует выбранные пользователем настройки рабочего пространства проекта.
    /// </summary>
    /// <param name="strategy">Выбранная пользователем стратегия представления.</param>
    /// <param name="templateId">Выбранный пользователем шаблон управления проектом.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="account">Аккаунт.</param>
    /// <param name="projectManagementName">Название в управлении проектом.</param>
    /// <param name="projectManagementNamePrefix">Префикс названия в управлении проектом.</param>
    /// <returns>Выходная модель.</returns>
    Task<ConfigSpaceSettingOutput> CommitSpaceSettingsAsync(string strategy, int templateId, long projectId,
        string account, string projectManagementName, string projectManagementNamePrefix);

    /// <summary>
    /// Метод получает Id проекта, который был ранее выбран пользователем для перехода к управлению проектом.
    /// Необходимо для построения ссылки в рабочее пространство проекта.
    /// </summary>
    /// <returns>Выходная модель.</returns>
    Task<ConfigSpaceSettingOutput> GetBuildProjectSpaceSettingsAsync(string account);
}