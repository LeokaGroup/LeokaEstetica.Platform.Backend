using LeokaEstetica.Platform.Models.Dto.Output.Metrics;

namespace LeokaEstetica.Platform.Diagnostics.Abstractions.Metrics;

/// <summary>
/// Абстракция сервиса метрик проектов.
/// </summary>
public interface IProjectMetricsService
{
    /// <summary>
    /// Метод получает последние 5 комментариев к проектам.
    /// Проекты не повторяются.
    /// </summary>
    /// <returns>Список комментариев.</returns>
    Task<IEnumerable<LastProjectCommentsOutput>> GetLastProjectCommentsAsync();
}