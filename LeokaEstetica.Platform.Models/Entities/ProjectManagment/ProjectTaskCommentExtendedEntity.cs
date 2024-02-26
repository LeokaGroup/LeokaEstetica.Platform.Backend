namespace LeokaEstetica.Platform.Models.Entities.ProjectManagment;

/// <summary>
/// Класс сущности расширяющей сущность комментариев задачи.
/// </summary>
public class ProjectTaskCommentExtendedEntity : ProjectTaskCommentEntity
{
    /// <summary>
    /// Префикс номера задачи.
    /// </summary>
    public string TaskIdPrefix { get; set; }

    /// <summary>
    /// ФИО или почта или логин пользователя, который добавил или изменил комментарий.
    /// </summary>
    public string UserName { get; set; }
}