namespace LeokaEstetica.Platform.ProjectManagment.ValidationModels;

/// <summary>
/// Класс валидации статусов задачи проекта.
/// </summary>
public class GetTaskStatusValidationModel
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    public GetTaskStatusValidationModel(long projectId)
    {
        ProjectId = projectId;
    }

    /// <summary>
    /// Id проекта.
    /// </summary>
    public long ProjectId { get; }
}