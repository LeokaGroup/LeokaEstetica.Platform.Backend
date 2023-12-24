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
    Task CommitSpaceSettingsAsync(string strategy, int templateId, long projectId, string account);
}