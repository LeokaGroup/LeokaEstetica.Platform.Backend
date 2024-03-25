using LeokaEstetica.Platform.Models.Dto.Base.ProjectManagement;

namespace LeokaEstetica.Platform.Models.Dto.Output.ProjectManagment;

/// <summary>
/// Класс выходной модели эпика, в который входит задача.
/// </summary>
public class EpicTaskOutput : BaseEpicOutput
{
    /// <summary>
    /// Id эпика в рамках проекта.
    /// </summary>
    public long ProjectEpicId { get; set; }
    
    /// <summary>
    /// Префикс номера задачи.
    /// </summary>
    public string TaskIdPrefix { get; set; }

    /// <summary>
    /// Id задачи в рамках проекта вместе с префиксом.
    /// </summary>
    public string FullProjectEpicId => string.Concat(TaskIdPrefix + "-", ProjectEpicId);
}