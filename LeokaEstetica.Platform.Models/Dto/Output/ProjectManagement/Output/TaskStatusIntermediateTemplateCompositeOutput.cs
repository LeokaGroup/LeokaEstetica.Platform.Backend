using LeokaEstetica.Platform.Models.Entities.Template;

namespace LeokaEstetica.Platform.Models.Dto.ProjectManagement.Output;

/// <summary>
/// Класс композитной выходной модели статусов шаблона.
/// </summary>
public class TaskStatusIntermediateTemplateCompositeOutput : ProjectManagmentTaskStatusIntermediateTemplateEntity
{
    /// <summary>
    /// Id статуса задачи, на которым мапится StatusId.
    /// </summary>
    public long TaskStatusId { get; set; }
}