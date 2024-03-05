using LeokaEstetica.Platform.Models.Entities.ProjectManagment;

namespace LeokaEstetica.Platform.Models.Dto.Output.ProjectManagment;

/// <summary>
/// Класс выходной модели эпика.
/// </summary>
public class EpicOutput : EpicEntity
{
    /// <summary>
    /// Префикс номера задачи.
    /// </summary>
    public string TaskIdPrefix { get; set; }

    /// <summary>
    /// Id задачи в рамках проекта вместе с префиксом.
    /// </summary>
    public string FullProjectTaskId => string.Concat(TaskIdPrefix + "-", ProjectTaskId);
}