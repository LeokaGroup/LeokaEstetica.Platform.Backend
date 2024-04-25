using LeokaEstetica.Platform.Models.Entities.ProjectManagment;

namespace LeokaEstetica.Platform.Services.Factors;

/// <summary>
/// Класс факторки создает сущность для метки (тега) задач пользователя.
/// </summary>
public static class CreateUserTaskTagFactory
{
    /// <summary>
    /// Метод создает и наполняет сущность метки (тега) проекта.
    /// </summary>
    /// <param name="tagName">Название тега.</param>
    /// <param name="tagDescription">Описание тега.</param>
    /// <param name="tagSysName">Системное название тега.</param>
    /// <param name="maxPosition">Максимальная позиция тега.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Сущность с данными.</returns>
    public static ProjectTagEntity CreateProjectTag(string tagName, string tagDescription, string tagSysName,
        int maxPosition, long projectId)
    {
        if (string.IsNullOrWhiteSpace(tagDescription))
        {
            tagDescription = null;
        }
        
        var result = new ProjectTagEntity(tagName,tagSysName)
        {
            TagDescription = tagDescription,
            Position = maxPosition,
            ProjectId = projectId
        };

        return result;
    }
}