using LeokaEstetica.Platform.Models.Entities.ProjectManagment;

namespace LeokaEstetica.Platform.Services.Factors;

/// <summary>
/// Класс факторки создает сущность для метки (тега) задач пользователя.
/// </summary>
public static class CreateUserTaskTagFactory
{
    /// <summary>
    /// Метод создает и наполняет сущность метки (тега) задач пользователя.
    /// </summary>
    /// <param name="tagName">Название тега.</param>
    /// <param name="tagDescription">Описание тега.</param>
    /// <param name="tagSysName">Системное название тега.</param>
    /// <param name="maxPosition">Максимальная позиция тега.</param>
    /// <returns>Сущность с данными.</returns>
    public static UserTaskTagEntity CreateUserTaskTag(string tagName, string tagDescription, string tagSysName,
        long userId, int maxPosition)
    {
        if (string.IsNullOrWhiteSpace(tagDescription))
        {
            tagDescription = null;
        }
        
        var result = new UserTaskTagEntity
        {
            TagName = tagName,
            TagSysName = tagSysName,
            TagDescription = tagDescription,
            UserId = userId,
            Position = maxPosition
        };

        return result;
    }
}