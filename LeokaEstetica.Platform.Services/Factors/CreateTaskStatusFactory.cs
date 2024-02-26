using LeokaEstetica.Platform.Models.Entities.Template;

namespace LeokaEstetica.Platform.Services.Factors;

/// <summary>
/// Класс факторки создания статуса.
/// </summary>
public static class CreateTaskStatusFactory
{
    /// <summary>
    /// Метод создает и наполняет сущность кастомного статуса пользователя.
    /// </summary>
    /// <param name="statusName">Название кастомного статуса пользователя.</param>
    /// <param name="statusSysName">Системное название кастомного статуса пользователя.</param>
    /// <param name="position"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    public static ProjectManagementUserStatuseTemplateEntity CreateUserStatuseTemplate(string statusName,
        string statusSysName, int position, long userId, string statusDescription)
    {
        if (string.IsNullOrEmpty(statusDescription))
        {
            statusDescription = null;
        }
        
        var result = new ProjectManagementUserStatuseTemplateEntity
        {
            StatusName = statusName,
            StatusSysName = statusSysName,
            UserId = userId,
            Position = position,
            StatusDescription = statusDescription
        };

        return result;
    }
}