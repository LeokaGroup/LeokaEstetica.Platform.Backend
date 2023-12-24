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
    Task CommitSpaceSettingsAsync(string strategy, int templateId, long projectId, long userId);
}