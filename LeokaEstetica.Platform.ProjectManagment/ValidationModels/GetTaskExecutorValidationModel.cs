namespace LeokaEstetica.Platform.ProjectManagment.ValidationModels;

/// <summary>
/// Класс валидации исполнителей задачи проекта.
/// </summary>
public class GetTaskExecutorValidationModel
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    public GetTaskExecutorValidationModel(long projectId)
    {
        ProjectId = projectId;
    }

    /// <summary>
    /// Id проекта.
    /// </summary>
    public long ProjectId { get; }
}