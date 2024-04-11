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
    /// <param name="strategy">Стратегия представления.</param>
    /// <param name="templateId">Id шаблона.</param>
    public GetConfigurationValidationModel(long projectId, string strategy, int templateId)
    {
        ProjectId = projectId;
        Strategy = strategy;
        TemplateId = templateId;
    }

    /// <summary>
    /// Id проекта.
    /// </summary>
    public long ProjectId { get; }

    /// <summary>
    /// Стратегия представления.
    /// </summary>
    public string Strategy { get; }

    /// <summary>
    /// Id шаблона.
    /// </summary>
    public int TemplateId { get; }
}