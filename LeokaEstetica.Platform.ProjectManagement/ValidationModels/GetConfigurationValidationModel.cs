namespace LeokaEstetica.Platform.ProjectManagment.ValidationModels;

/// <summary>
/// Класс валидации параметров рабочего пространства.
/// </summary>
public class GetConfigurationValidationModel
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    public GetConfigurationValidationModel(long projectId)
    {
        ProjectId = projectId;
    }

    /// <summary>
    /// Id проекта.
    /// </summary>
    public long ProjectId { get; }
}