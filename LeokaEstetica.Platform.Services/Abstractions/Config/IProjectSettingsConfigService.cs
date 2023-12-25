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
    /// <param name="configSpaceSettingInput">Входная модель.</param>
    Task<ConfigSpaceSettingOutput> CommitSpaceSettingsAsync(string strategy, int templateId, long projectId,
        string account);

    /// <summary>
    /// Метод получает Id проекта, который был ранее выбран пользователем для перехода к управлению проектом.
    /// Необходимо для построения ссылки в рабочее пространство проекта.
    /// </summary>
    /// <returns>Выходная модель.</returns>
    Task<ConfigSpaceSettingOutput> GetBuildProjectSpaceSettingsAsync(string account);
}