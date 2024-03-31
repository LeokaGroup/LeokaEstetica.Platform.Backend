using LeokaEstetica.Platform.Models.Dto.Base.ProjectManagement;

namespace LeokaEstetica.Platform.Models.Dto.Output.ProjectManagment;

/// <summary>
/// Класс выходной модели статусов задачи.
/// </summary>
public class TaskStatusOutput : BaseTaskStatusOutput
{
    /// <summary>
    /// Id шаблона.
    /// </summary>
    public int TemplateId { get; set; }
}